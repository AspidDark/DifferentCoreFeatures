using DPSService.DB;
using DPSService.Options;
using DPSService.ReportHelper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
//using Quartz;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace DPSService.Jobs
{
    // public interface ICreateReportJob : IJob { }
    public sealed class CreateReportJob //: ICreateReportJob
    {
        public const string creditAnnuities = "CreditAnnuities";
        public const string credit = "Credit";
        // public const string installmentFileName = "Installment2-2.xlsx";
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ExcelTemplaesOptions _excelTemplaesOptions;
        private readonly CalculatorsSavePathOptions _calculatorsSavePathOptions;
        private readonly IConfiguration _configuration;

        const int additionalRow = 3;
        public CreateReportJob(IConfiguration configuration, ExcelTemplaesOptions excelTemplaesOptions,
             IServiceScopeFactory scopeFactory, CalculatorsSavePathOptions calculatorsSavePathOptions)
        {
            _configuration = configuration;
            _scopeFactory = scopeFactory;
            _excelTemplaesOptions = excelTemplaesOptions;
            _calculatorsSavePathOptions = calculatorsSavePathOptions;
        }
        public async Task Execute()
        {
            string logPath = Path.Combine(_configuration.GetValue<string>("ReportLoggerPath"),
             $"{DateTime.Now:yyyy.MM.dd}-upload.log");
            using var _logger = CreateLogger(logPath, LogEventLevel.Information);
            _logger.Information("Старт процесса: {time}", DateTimeOffset.Now);
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    List<Guid> newContracts = dbContext.GraceContacts.AsNoTracking().Where(x => x.UpdateDTTM.Date == DateTime.Now.Date).Select(x => x.ContractId).ToList();

                    List<Guid> newPayments = dbContext.Payments.AsNoTracking().Where(x => x.ModifiedOn.Date == DateTime.Now.Date).Select(x => x.ContractId).ToList();

                    IEnumerable<Guid> allContracts = newContracts.Union(newPayments).Distinct();

                    var graceContractList = dbContext.GraceContacts.AsNoTracking().Where(x => allContracts.Contains(x.ContractId)).ToList();
                    var paymenstList = dbContext.Payments.AsNoTracking().Where(x => allContracts.Contains(x.ContractId)).ToList();

                    //List<SupplementaryAgreement> supplementaryAgreements = new List<SupplementaryAgreement>();
                    //List<InterestCharge> interestCharges = new List<InterestCharge>();
                    foreach (var item in allContracts)
                    {
                        var graceContract = graceContractList.FirstOrDefault(x => x.ContractId == item);
                        var payments = paymenstList.Where(x => x.ContractId == item);
                        Dictionary<string, decimal> amounts = new Dictionary<string, decimal>();
                        switch (graceContract.BankingServiceType)
                        {
                            case creditAnnuities:
                                try
                                {
                                    Workbook installmentCalculator = new Workbook();
                                    installmentCalculator.LoadFromFile(_excelTemplaesOptions.AnnualTemplatePath);
                                    Worksheet installmentCalculatorSheet = installmentCalculator.Worksheets[0];

                                    _logger.Information($"Для договора {item.ToString()} старт формирования калькулятора и внесения в базу отчетов");
                                    DataCounter insallmentCounter = new DataCounter(graceContract, installmentCalculatorSheet, _logger);
                                    installmentCalculatorSheet.SetCellValue(3, 1, graceContract.BankingServiceName); //Наименование продукта
                                    installmentCalculatorSheet.SetCellValue(3, 2, insallmentCounter.GetQuarter(graceContract.GracePeriodStartDate)); //Квартал
                                    installmentCalculatorSheet.SetCellValue(5, 2, graceContract.PrincipalPayable.ToString()); //Сумма ОД займа
                                    installmentCalculatorSheet.SetCellValue(6, 2, graceContract.LoanPeriodDaily.ToString()); //Срок
                                    installmentCalculatorSheet.SetCellValue(7, 2, graceContract.LoanRateDay.ToString("0.00", System.Globalization.CultureInfo.GetCultureInfo("ru-RU")) + "%"); //Ставка
                                    installmentCalculatorSheet.SetCellValue(8, 2, graceContract.TermVariance.ToString()); //Срок между платежами  

                                    installmentCalculatorSheet.Range["C4"].DateTimeValue = graceContract.ContractSignedOn.Date;
                                    installmentCalculatorSheet.Range["C21"].DateTimeValue = graceContract.GracePeriodStartDate.Date;
                                    installmentCalculatorSheet.Range["C22"].DateTimeValue = graceContract.GracePeriodEndDate.Date;
                                    installmentCalculatorSheet.Range["B31"].DateTimeValue = DateTime.Now.Date;

                                    installmentCalculatorSheet.SetCellValue(10, 2, graceContract.PaymentAmount.ToString()); //Размер платежа на момент КК
                                    installmentCalculatorSheet.SetCellValue(11, 2, graceContract.PrincipalPzPayable.ToString()); //Остаток ОД на момент КК
                                    installmentCalculatorSheet.SetCellValue(12, 2, (graceContract.GracePeriodStartDate - graceContract.ContractSignedOn).TotalDays.ToString()); //День в графике платежа на начало КК
                                    installmentCalculatorSheet.SetCellValue(13, 2, graceContract.PercentPzPayable.ToString()); //Задолженность по процентам
                                    installmentCalculatorSheet.SetCellValue(14, 2, graceContract.PenaltyPzPayable.ToString()); //Задолженность по пенни
                                    installmentCalculatorSheet.SetCellValue(15, 2, graceContract.ContractStatus.ToString()); //Займ в просрочке на начало КК?
                                    installmentCalculatorSheet.SetCellValue(16, 2, insallmentCounter.GraceContractAndPaymentDaysEqual().ToString()); //День выхода на КК и день платежа совпадают?
                                    installmentCalculatorSheet.SetCellValue(17, 2, graceContract.TotalPercentPzPayable.ToString()); //Выплачено процентов на момент КК
                                    installmentCalculatorSheet.SetCellValue(3, 24, graceContract.Balance.ToString()); //Баланс

                                    //https://jira.bistrodengi.ru/browse/SMIS-4088
                                    installmentCalculatorSheet.SetCellValue(14, 5, graceContract.PaymentPenalty.ToString()); //Баланс
                                    installmentCalculatorSheet.SetCellValue(11, 5, graceContract.PrincipalPzPayableCollection.ToString()); //Баланс
                                    installmentCalculatorSheet.SetCellValue(13, 5, graceContract.PercentPzPayableCollection.ToString()); //Баланс

                                    //сохранили т.к при чтении глючит.
                                    installmentCalculator.CalculateAllValue();
                                    string currentInstallmentPath = $"{_calculatorsSavePathOptions.Installment}{graceContract.ContractNumber}.xlsx";
                                    installmentCalculator.SaveToFile(currentInstallmentPath);
                                    installmentCalculator.Dispose();


                                    //забираем Для записи оплат
                                    Workbook installmentCalculator1 = new Workbook();
                                    installmentCalculator1.LoadFromFile(currentInstallmentPath);
                                    Worksheet installmentCalculatorSheet1 = installmentCalculator1.Worksheets[0];

                                    DataCounter dataCounterForpayments = new DataCounter(graceContract, installmentCalculatorSheet1, _logger);

                                    foreach (var payment in payments)
                                    {
                                        //Если это Оптата
                                        if (payment.Type == 1)
                                        {
                                            int rowIndex = 0;
                                            int colimnIndex = 0;

                                            if (dataCounterForpayments.BeforeGracePeriod(payment.Date))
                                            {
                                                continue;
                                            }

                                            int graceContractDurationInDays = (int)(graceContract.GracePeriodEndDate - graceContract.GracePeriodStartDate).TotalDays;

                                            if (dataCounterForpayments.AfterGracePeriod(payment.Date))
                                            {
                                                colimnIndex = 23;
                                                //Проверить, нужно ли вычитать единицу
                                                rowIndex = (int)(payment.Date - graceContract.GracePeriodEndDate).TotalDays + additionalRow - 1;
                                            }

                                            if (dataCounterForpayments.PaymentDueGraceContractPeriod(payment.Date))
                                            {
                                                colimnIndex = 34;
                                                rowIndex = (int)(payment.Date - graceContract.GracePeriodStartDate).TotalDays + additionalRow;
                                            }

                                            if (dataCounterForpayments.AfterContractPeriod(payment.Date))
                                            {
                                                colimnIndex = 44;
                                                rowIndex = (int)(payment.Date - graceContract.ContractSignedOn.AddDays(graceContract.LoanPeriodDaily).
                                                AddDays(graceContractDurationInDays)).TotalDays + additionalRow;
                                            }
                                            string dictionaryKey = $"{rowIndex}|{colimnIndex}";
                                            if (amounts.ContainsKey(dictionaryKey))
                                            {
                                                amounts[dictionaryKey] += payment.Amount;
                                            }
                                            else
                                            {
                                                amounts.Add(dictionaryKey, payment.Amount);
                                            }
                                            installmentCalculatorSheet1.SetCellValue(rowIndex, colimnIndex, amounts[dictionaryKey].ToString());
                                        }
                                        else
                                        {
                                            int dictionaryColumnIndex = 23;
                                            int rowIndex = 0;
                                            int colimnIndex = 25;

                                            int graceContractDurationInDays = (int)(graceContract.GracePeriodEndDate - graceContract.GracePeriodStartDate).TotalDays;
                                            if (dataCounterForpayments.AfterGracePeriod(payment.Date))
                                            {
                                                //Проверить, нужно ли вычитать единицу
                                                rowIndex = (int)(payment.Date - graceContract.GracePeriodEndDate).TotalDays + additionalRow - 1;
                                            }
                                            string dictionaryKey = $"{rowIndex}|{dictionaryColumnIndex}";

                                            installmentCalculatorSheet1.SetCellValue(rowIndex, colimnIndex, payment.Amount.ToString());

                                            if (amounts.ContainsKey(dictionaryKey))
                                            {
                                                amounts[dictionaryKey] += payment.Amount;
                                            }
                                            else
                                            {
                                                amounts.Add(dictionaryKey, payment.Amount);
                                            }
                                            installmentCalculatorSheet1.SetCellValue(rowIndex, colimnIndex, amounts[dictionaryKey].ToString());

                                        }
                                    }
                                    var currentCultureInfoInst = System.Threading.Thread.CurrentThread.CurrentCulture;
                                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("ru-RU");

                                    installmentCalculator1.CalculateAllValue();
                                    installmentCalculator1.SaveToFile(currentInstallmentPath);
                                    installmentCalculator1.Dispose();
                                    ////////  Это сохранение рассчетов
                                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCultureInfoInst;

                                    Workbook installmentCalculator2 = new Workbook();
                                    installmentCalculator2.LoadFromFile(currentInstallmentPath);
                                    Worksheet installmentCalculatorSheet2 = installmentCalculator2.Worksheets[0];
                                    DataCounter dataCounterInstallmentGeter = new DataCounter(graceContract, installmentCalculatorSheet2, _logger);

                                    decimal currentDebt = -1;
                                    DateTime? debtClosedOndate = null;
                                    decimal.TryParse(DataCounter.ShortString(installmentCalculatorSheet2.Range["B32"].FormulaValue.ToString().Replace(".", ",")), NumberStyles.Any, new CultureInfo("ru-RU"), out currentDebt);
                                    if (currentDebt == 0)
                                    {
                                        ContractData contractData = dbContext.ContractData.FirstOrDefault(x => x.ContractId == item);
                                        if (contractData == null)
                                        {
                                            dbContext.ContractData.Add(new ContractData { Id = Guid.NewGuid(), ContractId = item, ContractClosedOn = DateTime.Now });
                                            dbContext.SaveChanges();
                                            debtClosedOndate = DateTime.Now;
                                        }
                                        else
                                        {
                                            debtClosedOndate = contractData.ContractClosedOn;
                                        }
                                    }

                                    _logger.Information($"старт наполнения Данных Installment SupplementaryAgreement для {graceContract.ContractNumber}");

                                    SupplementaryAgreement supplementaryAgreement = new SupplementaryAgreement
                                    {
                                        Id = Guid.NewGuid(),
                                        ContractId = item,
                                        ContractNumberABS = graceContract.ContractNumberABS,
                                        GracePeriodStartDate = graceContract.GracePeriodStartDate
                                    };
                                    try
                                    {
                                        string newContractEndDateString = installmentCalculatorSheet2.Range[$"B26"].FormulaValue.ToString();
                                        DateTime.TryParse(newContractEndDateString, out DateTime newContractEndDate);
                                        supplementaryAgreement.NewContractEndDate = newContractEndDate;
                                        // supplementaryAgreements.Add(supplementaryAgreement);

                                        //Удаляем старые SupplementaryAgreements
                                        var supplementaryAgreementInsToDelete = dbContext.SupplementaryAgreements.
                                           FirstOrDefault(x => x.ContractNumberABS == supplementaryAgreement.ContractNumberABS);
                                        if (supplementaryAgreementInsToDelete != null)
                                        {
                                            dbContext.SupplementaryAgreements.Remove(supplementaryAgreementInsToDelete);
                                        }
                                        //добавляем новые SupplementaryAgreements
                                        dbContext.SupplementaryAgreements.Add(supplementaryAgreement);
                                        dbContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(ex, $"Ошибка формирования Installment SupplementaryAgreements | {graceContract.ContractNumber} ошибка {ex.InnerException?.ToString()}");
                                    }
                                    _logger.Information($"старт удаления старых Installment данных InterestCharges для {graceContract.ContractNumber}");
                                    //Удаляем старые InterestCharges
                                    var interestChargesInsTodelete = dbContext.InterestCharges.Where(x => x.ContractNumberABS == supplementaryAgreement.ContractNumberABS);
                                    if (interestChargesInsTodelete.Any())
                                    {
                                        foreach (var interestChargePdl in interestChargesInsTodelete)
                                        {
                                            dbContext.Remove(interestChargePdl);
                                        }
                                        dbContext.SaveChanges();
                                    }

                                    _logger.Information($"старт создания Installment InterestCharges данных InterestCharges для {graceContract.ContractNumber}");

                                    Dictionary<YearAndMonth, decimal> sumAndDateDtos = dataCounterInstallmentGeter.GetMonthSum();
                                    foreach (var sumAndDate in sumAndDateDtos)
                                    {
                                        try
                                        {
                                            YearAndMonth yearAndMonth = sumAndDate.Key;
                                            DateTime dateTime = new DateTime(yearAndMonth.year, yearAndMonth.month,
                                                DateTime.DaysInMonth(yearAndMonth.year, yearAndMonth.month));

                                            _logger.Information("старт создания Installment InterestCharge START");

                                            decimal correctContractAmount = 0;
                                            if (sumAndDate.Value > 0)
                                            {
                                                correctContractAmount = sumAndDate.Value;
                                            }

                                            InterestCharge interestCharge = new InterestCharge
                                            {
                                                Id = Guid.NewGuid(),
                                                ContractId = item,
                                                ContractNumberABS = graceContract.ContractNumberABS,
                                                ContractType = 1,
                                                ContractKind = 118,
                                                ContractAmount = correctContractAmount,
                                                LastDayOfMonth = dateTime
                                            };

                                            _logger.Information($"старт создания Installment InterestCharge START LastDayOfMonth= {interestCharge.LastDayOfMonth} | ContractAmount= {interestCharge.ContractAmount}");
                                            //добавляем новые InterestCharges
                                            dbContext.InterestCharges.Add(interestCharge);

                                            dbContext.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error(ex, $" Запись Installment InterestCharge ошибка {graceContract.ContractNumber}");
                                        }

                                        // interestCharges.Add(interestCharge);
                                    }

                                    // получение из документа
                                    #region DWH report
                                    /* if (false)
                                     {
                                         CalculationResult calculationResult = new CalculationResult
                                         {
                                             Id = Guid.NewGuid(),
                                             ContractId = item,
                                             AccountID = graceContract.ContractNumberABS
                                         };

                                         // var tpm = DateTime.Now.Date.ToString("dd.MM.yyyy");
                                         int currentDateRow = dataCounter.GetRow("H", DateTime.Now.Date);///
                                         Payment lastPaymentFromBase = payments.OrderByDescending(x => x.Date).FirstOrDefault();
                                         int latsPaymentRowIndex = 0;/////////////////?????

                                         switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                         {
                                             case Periods.BeforeGrace:
                                                 //??
                                                 break;
                                             case Periods.InGrace:
                                                 latsPaymentRowIndex = dataCounter.GetRow("AE", lastPaymentFromBase.Date);
                                                 break;
                                             case Periods.AfterGrace:
                                                 int columnAA = dataCounter.RowsUpCountEmptyValueInColumns("AA", currentDateRow);///
                                                 int columnAB = dataCounter.RowsUpCountEmptyValueInColumns("AB", currentDateRow);///
                                                 int columnAC = dataCounter.RowsUpCountEmptyValueInColumns("AC", currentDateRow);///
                                                 int minColumn = new List<int> { columnAA, columnAB, columnAC }.Min();
                                                 latsPaymentRowIndex = currentDateRow - minColumn;
                                                 break;
                                             case Periods.AfterContract:
                                                 latsPaymentRowIndex = dataCounter.GetRow("AM", lastPaymentFromBase.Date);
                                                 break;
                                         }
                                         //v W Y  AH  AR
                                         //5 Account_Rating
                                         string accountRatingValueFromExcel = string.Empty;
                                         if ((DateTime?)null != null)
                                         {
                                             calculationResult.AccountRating = "13";
                                         }
                                         else
                                         {
                                             switch (dataCounter.GetPeriod(DateTime.Now))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //?
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.AccountRating = "00";
                                                     break;
                                                 case Periods.AfterGrace:
                                                     accountRatingValueFromExcel = installmentCalculatorSheet2.Range[$"Z{currentDateRow}"].FormulaValue.ToString();
                                                     calculationResult.AccountRating = accountRatingValueFromExcel switch
                                                     {
                                                         "1" => "52",
                                                         _ => "00",
                                                     };
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.AccountRating = "00";
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }

                                         //6 Date_Account_Rating
                                         if ((DateTime?)null != null)
                                         {
                                             calculationResult.DateAccountRating = (DateTime?)null ?? DateTime.Now;
                                         }
                                         else
                                         {
                                             switch (dataCounter.GetPeriod(DateTime.Now))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //?
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.DateAccountRating = graceContract.GracePeriodStartDate;
                                                     break;
                                                 case Periods.AfterGrace:
                                                     int rowsUpCount = dataCounter.RowsUpCount(accountRatingValueFromExcel, "Z", currentDateRow);

                                                     rowsUpCount = CorrectRowsCountValue(rowsUpCount, currentDateRow);

                                                     string contractChangeDate = installmentCalculatorSheet2.Range[$"H{currentDateRow - rowsUpCount}"].FormulaValue.ToString();

                                                     DateTime parsedRating = DateTime.MinValue;
                                                     DateTime.TryParse(contractChangeDate, out parsedRating);
                                                     calculationResult.DateAccountRating = parsedRating;
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.DateAccountRating = dataCounter.GetLoanEndDate();
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }
                                         //8 Balance
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 calculationResult.Balance = graceContract.Balance;
                                                 break;
                                             case Periods.InGrace:
                                                 calculationResult.Balance = graceContract.Balance;
                                                 break;
                                             case Periods.AfterGrace:
                                                 string currentBalance = installmentCalculatorSheet2.Range[$"X{currentDateRow}"].FormulaValue.ToString().Replace(".", ",");
                                                 if (decimal.TryParse(currentBalance, out decimal currenBalanceAmount))
                                                 {
                                                     if (currenBalanceAmount >= 0)
                                                     {
                                                         calculationResult.Balance = currenBalanceAmount;
                                                     }
                                                     else
                                                     {
                                                         calculationResult.Balance = 0;
                                                     }
                                                 }
                                                 break;
                                             case Periods.AfterContract:
                                                 int graceContractEndColumnIndex = dataCounter.GetRow("H", graceContract.GracePeriodEndDate);
                                                 string calculationBalance = installmentCalculatorSheet2.Range[$"X{graceContractEndColumnIndex}"].FormulaValue.ToString().Replace(".", ",");
                                                 decimal.TryParse(calculationBalance, out decimal calculationBalanceD);
                                                 if (calculationBalanceD >= 0)
                                                 {
                                                     calculationResult.Balance = calculationBalanceD;
                                                 }
                                                 else
                                                 {
                                                     calculationResult.Balance = 0;
                                                 }
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //9 Past_Due
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //?
                                                 break;
                                             case Periods.InGrace:
                                                 calculationResult.PastDue = 0;
                                                 break;
                                             case Periods.AfterGrace:
                                                 if (accountRatingValueFromExcel == "1")
                                                 {
                                                     string pastDueP1 = installmentCalculatorSheet2.Range[$"AD{currentDateRow}"].FormulaValue.ToString();
                                                     int pastDueCahngeAmount = dataCounter.RowsUpCount(pastDueP1, "AD", currentDateRow);

                                                     pastDueCahngeAmount = CorrectRowsCountValue(pastDueCahngeAmount, currentDateRow);

                                                     decimal qColumnSumm = dataCounter.SummColumnValues("Q", currentDateRow - pastDueCahngeAmount, pastDueCahngeAmount);
                                                     decimal lColumnSumm = dataCounter.SummColumnValues("L", currentDateRow - pastDueCahngeAmount, pastDueCahngeAmount);
                                                     calculationResult.PastDue = qColumnSumm + lColumnSumm;

                                                 }
                                                 else
                                                 {
                                                     if (accountRatingValueFromExcel == "0")
                                                         calculationResult.PastDue = 0;
                                                 }
                                                 break;
                                             case Periods.AfterContract:
                                                 //?
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //10  Next_Payment
                                         int graceContractDurationInDaysNextPayment = (int)(graceContract.GracePeriodEndDate - graceContract.GracePeriodStartDate).TotalDays;
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //?
                                                 break;
                                             case Periods.InGrace:
                                                 calculationResult.NextPaymentAmount = dataCounter.GetNextNotKeyValue("L", additionalRow);
                                                 break;
                                             case Periods.AfterGrace:
                                                 calculationResult.NextPaymentAmount = dataCounter.GetNextNotKeyValue("L", currentDateRow);
                                                 break;
                                             case Periods.AfterContract:
                                                 calculationResult.NextPaymentAmount = dataCounter.GetNextNotKeyValue("AP", currentDateRow);
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //11 MOP

                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 calculationResult.MOP = "Z";
                                                 break;
                                             case Periods.AfterGrace:
                                                 string mopCheckValue = installmentCalculatorSheet2.Range[$"Z{currentDateRow}"].FormulaValue.ToString();

                                                 if (mopCheckValue == "0")
                                                 {
                                                     calculationResult.MOP = "1";
                                                 }
                                                 else
                                                 {
                                                     int mopRowsUpCountA = dataCounter.RowsUpCount(mopCheckValue, "Z", currentDateRow);
                                                     mopRowsUpCountA = CorrectRowsCountValue(mopRowsUpCountA, currentDateRow);
                                                     if (dataCounter.IsInInterval(mopRowsUpCountA, 1, 7))
                                                     {
                                                         calculationResult.MOP = "B";
                                                     }
                                                     if (dataCounter.IsInInterval(mopRowsUpCountA, 8, 29))
                                                     {
                                                         calculationResult.MOP = "C";
                                                     }
                                                     if (dataCounter.IsInInterval(mopRowsUpCountA, 30, 59))
                                                     {
                                                         calculationResult.MOP = "2";
                                                     }
                                                     if (dataCounter.IsInInterval(mopRowsUpCountA, 60, 89))
                                                     {
                                                         calculationResult.MOP = "3";
                                                     }
                                                     if (dataCounter.IsInInterval(mopRowsUpCountA, 90, 119))
                                                     {
                                                         calculationResult.MOP = "4";
                                                     }
                                                     if (mopRowsUpCountA >= 120)
                                                     {
                                                         calculationResult.MOP = "5";
                                                     }

                                                 }
                                                 break;
                                             case Periods.AfterContract:
                                                 int mopRowsUpCount = dataCounter.RowsUpCount("1", "Z", graceContract.TermVariance + additionalRow);
                                                 mopRowsUpCount = CorrectRowsCountValue(mopRowsUpCount, currentDateRow);
                                                 if (dataCounter.IsInInterval(mopRowsUpCount, 1, 7))
                                                 {
                                                     calculationResult.MOP = "B";
                                                 }
                                                 if (dataCounter.IsInInterval(mopRowsUpCount, 8, 29))
                                                 {
                                                     calculationResult.MOP = "C";
                                                 }
                                                 if (dataCounter.IsInInterval(mopRowsUpCount, 30, 59))
                                                 {
                                                     calculationResult.MOP = "2";
                                                 }
                                                 if (dataCounter.IsInInterval(mopRowsUpCount, 60, 89))
                                                 {
                                                     calculationResult.MOP = "3";
                                                 }
                                                 if (dataCounter.IsInInterval(mopRowsUpCount, 90, 119))
                                                 {
                                                     calculationResult.MOP = "4";
                                                 }
                                                 if (mopRowsUpCount >= 120)
                                                 {
                                                     calculationResult.MOP = "5";
                                                 }
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //12 Date_of_Contract_Termination
                                         DateTime.TryParse(installmentCalculatorSheet2.Range["B26"].FormulaValue.ToString(), out DateTime contractTermination);
                                         calculationResult.DateofContractTermination = contractTermination;

                                         //13 Date_Payment_Due
                                         var dataPaymentDue = dbContext.ContractData.FirstOrDefault(x => x.ContractId == item);
                                         if (dataPaymentDue != null)
                                         {
                                             calculationResult.DatePaymentDue = dataPaymentDue.ContractClosedOn;
                                         }
                                         else
                                         {
                                             calculationResult.DatePaymentDue = contractTermination;
                                         }

                                         //14 Date_Interest_Payment_Due
                                         calculationResult.DateInterestPaymentDue = calculationResult.DatePaymentDue;

                                         //15 Amount_Outstanding
                                         decimal.TryParse(installmentCalculatorSheet2.Range["B32"].FormulaValue.ToString().Replace(".", ","), out decimal amountOutstending);
                                         calculationResult.AmountOutstanding = amountOutstending;

                                         //16 Complete_Obligations_Date
                                         calculationResult.CompleteObligationsDate = dataPaymentDue.ContractClosedOn;

                                         //17 Principal_Amount_Outstanding
                                         int lastPamentRow = dataCounter.GetRow("H", lastPaymentFromBase.Date);
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //
                                                     break;
                                                 case Periods.InGrace:
                                                     //Проверить использовать этот день или (этот день +1)
                                                     int graceLastdayIndex = dataCounter.GetRow("H", graceContract.GracePeriodEndDate.ToString());
                                                     calculationResult.PrincipalAmountOutstanding = installmentCalculatorSheet2.Range[$"K{graceLastdayIndex}"].FormulaValue.ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     calculationResult.PrincipalAmountOutstanding = installmentCalculatorSheet2.Range[$"K{lastPamentRow}"].FormulaValue.ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.PrincipalAmountOutstanding = dataCounter.GetLastNotEmptyValue("K");
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }
                                         else
                                         {
                                             calculationResult.PrincipalAmountOutstanding = graceContract.PrincipalPzPayable.ToString();
                                         }

                                         //18 Interest_Amount_Outstanding
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     calculationResult.InterestAmountOutstanding = graceContract.PercentPzPayable.ToString();
                                                     break;
                                                 case Periods.InGrace:
                                                     int lastPaymentRowIndex = dataCounter.GetRow("AE", lastPaymentFromBase.Date);
                                                     decimal gracePerodPercentageSum = dataCounter.SummColumnValues("AG", additionalRow, lastPaymentRowIndex - 3);
                                                     calculationResult.InterestAmountOutstanding = (graceContract.PercentPzPayable + gracePerodPercentageSum).ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     decimal gracePerodPercentageSumAfter = dataCounter.SummColumnValues("AG", additionalRow, 200);

                                                     decimal percentageSum = dataCounter.SummColumnValues("O", additionalRow, lastPamentRow - 3);
                                                     decimal percentagePayedSum = dataCounter.SummColumnValuesWithEmptyRows("AB", additionalRow, lastPamentRow - 3);
                                                     calculationResult.InterestAmountOutstanding = (gracePerodPercentageSumAfter + percentageSum - percentagePayedSum + graceContract.PercentPzPayable).ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     decimal gracePerodPercentageSumAfterContract = dataCounter.SummColumnValues("AG", 3, 200);

                                                     decimal percentageSumAfterContract = dataCounter.SummColumnValues("O", 3, 200);
                                                     decimal percentagePayedSumAfterContract = dataCounter.SummColumnValuesWithEmptyRows("AB", 3, 200);
                                                     calculationResult.InterestAmountOutstanding = (gracePerodPercentageSumAfterContract + percentageSumAfterContract - percentagePayedSumAfterContract + graceContract.PercentPzPayable).ToString();
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }

                                         //20 Principal_Amount_Past_Due
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //
                                                     break;
                                                 case Periods.InGrace:
                                                     int lastPamentRowIndex = dataCounter.GetRow("AE", lastPaymentFromBase.Date);
                                                     string debtDueGraceContract = installmentCalculatorSheet2.Range[$"AF{lastPamentRowIndex}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDueGraceContract, out decimal debtDueGraceContractDecimal);
                                                     int graceFirstdayIndex = dataCounter.GetRow("H", graceContract.GracePeriodStartDate.AddDays(-1));
                                                     string debtDuePaymentOrder = installmentCalculatorSheet2.Range[$"J{graceFirstdayIndex}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDuePaymentOrder, out decimal debtDuePaymentOrderDecimal);
                                                     calculationResult.InterestAmountOutstanding = (debtDueGraceContractDecimal - debtDuePaymentOrderDecimal).ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     string debtDuePaymentOrderGraceContract = installmentCalculatorSheet2.Range[$"K{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDuePaymentOrderGraceContract, out decimal debtDuePaymentOrderGraceContractD);
                                                     string debtDuePaymentOrderLast = installmentCalculatorSheet2.Range[$"J{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDuePaymentOrderLast, out decimal debtDuePaymentOrderLastD);
                                                     calculationResult.InterestAmountOutstanding = (debtDuePaymentOrderGraceContractD - debtDuePaymentOrderLastD).ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     string debtDuePaymentOrderGraceContractA = installmentCalculatorSheet2.Range[$"K{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDuePaymentOrderGraceContractA, out decimal debtDuePaymentOrderGraceContractDA);
                                                     string debtDuePaymentOrderLastA = installmentCalculatorSheet2.Range[$"J{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(debtDuePaymentOrderLastA, out decimal debtDuePaymentOrderLastDA);
                                                     calculationResult.InterestAmountOutstanding = (debtDuePaymentOrderGraceContractDA - debtDuePaymentOrderLastDA).ToString();
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }

                                         //21 Interest_Amount_Past_Due
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     break;
                                                 case Periods.InGrace:
                                                     int graceFirstdayIndex = dataCounter.GetRow("H", graceContract.GracePeriodStartDate.AddDays(-1));
                                                     decimal percentageSumDueGrace = dataCounter.SummColumnValues("O", additionalRow, graceFirstdayIndex - 3);
                                                     int lastPamentRowIndex = dataCounter.GetRow("AE", DateTime.Now.Date);
                                                     decimal gracePerodPercentageSum = dataCounter.SummColumnValues("AG", additionalRow, lastPamentRowIndex - 3 - 1);
                                                     decimal percentagePayedSumDueGrace = dataCounter.SummColumnValuesWithEmptyRows("AB", additionalRow, graceFirstdayIndex - 3);
                                                     calculationResult.InterestAmountPastDue = (percentageSumDueGrace + gracePerodPercentageSum - percentagePayedSumDueGrace).ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     decimal percentageSum = dataCounter.SummColumnValues("O", additionalRow, lastPamentRow - 3 - 1);
                                                     decimal percentagePayedSum = dataCounter.SummColumnValuesWithEmptyRows("AB", additionalRow, lastPamentRow - 3 - 1);
                                                     calculationResult.InterestAmountPastDue = (percentageSum - percentagePayedSum).ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     //?
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }


                                         //22 Other_Amount_Past_Due
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.OtherAmountPastDue = graceContract.PercentPzPayable.ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     calculationResult.OtherAmountPastDue = installmentCalculatorSheet2.Range[$"R{lastPamentRow}"].FormulaValue.ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     //?
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }

                                         //23 Grace_Period_Start_Date
                                         calculationResult.GracePeriodStartDate = graceContract.GracePeriodStartDate;

                                         //24 Grace_Period_End_Date
                                         calculationResult.GracePeriodEndDate = graceContract.GracePeriodEndDate;

                                         //25 Date _Grace_Period_Declined

                                         //26 Grace_Period_Reason
                                         calculationResult.GracePeriodReason = "02";

                                         //27 redit_cred_date_missedpayout 
                                         int daysDiff = (graceContract.GracePeriodStartDate - graceContract.ContractSignedOn).Days;
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 if (graceContract.ContractStatus == 1)
                                                 {
                                                     int i = 0;
                                                     while (i < daysDiff)
                                                     {
                                                         i += graceContract.TermVariance;
                                                     }
                                                     calculationResult.CreditCredDateMissedpayout = graceContract.ContractSignedOn.AddDays(i);
                                                 }
                                                 break;
                                             case Periods.AfterGrace:
                                                 if (calculationResult.AccountRating == "52")
                                                 {
                                                     string nextPayment = dataCounter.GetLastNotEmptyValue("L", currentDateRow);
                                                     int nextPaymentRowIndex = dataCounter.GetRow("L", nextPayment, currentDateRow);
                                                     DateTime paymentDate = DateTime.MinValue;
                                                     DateTime.TryParse(installmentCalculatorSheet2.Range[$"H{nextPaymentRowIndex}"].FormulaValue.ToString(), out paymentDate);
                                                     calculationResult.CreditCredDateMissedpayout = paymentDate;
                                                 }
                                                 break;
                                             case Periods.AfterContract:
                                                 string odSum = installmentCalculatorSheet2.Range[$"J{graceContract.LoanPeriodDaily - daysDiff - 1}"].FormulaValue.ToString();
                                                 if (odSum != "0")
                                                 {
                                                     calculationResult.CreditCredDateMissedpayout = dataCounter.GetLoanEndDate();
                                                 }
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //28 op_cred_sum_payout 
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(DateTime.Now))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.OpCredSumPayout = lastPaymentFromBase.Amount.ToString();
                                                     break;
                                                 case Periods.AfterGrace:
                                                     calculationResult.OpCredSumPayout = installmentCalculatorSheet2.Range[$"AA{lastPamentRow}"].FormulaValue.ToString();
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.OpCredSumPayout = lastPaymentFromBase.Amount.ToString();
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }


                                         //29 op_cred_sum_paid
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 string firstDayGraceSum = installmentCalculatorSheet2.Range[$"K3"].FormulaValue.ToString().Replace(".", ",");
                                                 decimal.TryParse(firstDayGraceSum, out decimal firstDayGraceSumD);
                                                 calculationResult.OpCredSumPaid = graceContract.PrincipalPayable - firstDayGraceSumD;
                                                 break;
                                             case Periods.AfterGrace:
                                                 string firstDayGraceSumA = installmentCalculatorSheet2.Range[$"K{currentDateRow}"].FormulaValue.ToString().Replace(".", ",");
                                                 decimal.TryParse(firstDayGraceSumA, out decimal firstDayGraceSumDA);
                                                 calculationResult.OpCredSumPaid = graceContract.PrincipalPayable - firstDayGraceSumDA;
                                                 break;
                                             case Periods.AfterContract:
                                                 string firstDayGraceSumC = installmentCalculatorSheet2.Range[$"B33"].FormulaValue.ToString().Replace(".", ",");
                                                 decimal.TryParse(firstDayGraceSumC, out decimal firstDayGraceSumDC);
                                                 calculationResult.OpCredSumPaid = graceContract.PrincipalPayable - firstDayGraceSumDC;
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //30 op_cred_date_overdue
                                         int rowsUpCountDateOverdue = dataCounter.RowsUpCount(accountRatingValueFromExcel, "Z", currentDateRow);

                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 //?
                                                 break;
                                             case Periods.AfterGrace:
                                                 if (rowsUpCountDateOverdue == -1)
                                                 {
                                                     calculationResult.OpCredDateOverdue = graceContract.GracePeriodEndDate;
                                                     break;
                                                 }
                                                 switch (accountRatingValueFromExcel)
                                                 {
                                                     case "1":
                                                         calculationResult.OpCredDateOverdue = DateTime.Now.AddDays(-rowsUpCountDateOverdue);
                                                         break;
                                                 }
                                                 break;
                                             case Periods.AfterContract:
                                                 //
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //31 op_cred_sum_nextpayout
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 calculationResult.OpCredSumNextpayout = dataCounter.NextNotNullValue("M", additionalRow);
                                                 break;
                                             case Periods.AfterGrace:
                                                 calculationResult.OpCredSumNextpayout = dataCounter.NextNotNullValue("M", currentDateRow);
                                                 break;
                                             case Periods.AfterContract:
                                                 calculationResult.OpCredSumNextpayout = dataCounter.NextNotNullValue("AP", currentDateRow);
                                                 break;
                                             default:
                                                 break;
                                         }


                                         //32 op_cred_date_nextpayout
                                         switch (dataCounter.GetPeriod(DateTime.Now))
                                         {
                                             case Periods.BeforeGrace:
                                                 //
                                                 break;
                                             case Periods.InGrace:
                                                 int nextNotEmptyRowIndex = dataCounter.GetNextNotEmptyRowIndex("M", additionalRow);
                                                 DateTime.TryParse(installmentCalculatorSheet2.Range[$"H{nextNotEmptyRowIndex}"].FormulaValue.ToString(), out DateTime nextNotEmptyRowIndexHDate);
                                                 calculationResult.OpCredDateNextpayout = nextNotEmptyRowIndexHDate;
                                                 break;
                                             case Periods.AfterGrace:
                                                 int nextNotEmptyRowIndexA = dataCounter.GetNextNotEmptyRowIndex("M", currentDateRow);
                                                 DateTime.TryParse(installmentCalculatorSheet2.Range[$"H{nextNotEmptyRowIndexA}"].FormulaValue.ToString(), out DateTime nextNotEmptyRowIndexHDateA);
                                                 calculationResult.OpCredDateNextpayout = nextNotEmptyRowIndexHDateA;
                                                 break;
                                             case Periods.AfterContract:
                                                 int nextNotEmptyRowIndexAF = dataCounter.GetNextNotEmptyRowIndex("AP", currentDateRow);
                                                 DateTime.TryParse(installmentCalculatorSheet2.Range[$"H{nextNotEmptyRowIndexAF}"].FormulaValue.ToString(), out DateTime nextNotEmptyRowIndexHDateAF);
                                                 calculationResult.OpCredDateNextpayout = nextNotEmptyRowIndexHDateAF;
                                                 break;
                                             default:
                                                 break;
                                         }

                                         //33 ta_cred_sum_payout 

                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(DateTime.Now))
                                             {
                                                 case Periods.BeforeGrace:
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.TaCredSumPayout = lastPaymentFromBase.Amount;
                                                     break;
                                                 case Periods.AfterGrace:
                                                     string realPayedValue = installmentCalculatorSheet2.Range[$"AA{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(realPayedValue, out decimal realPayedValueD);
                                                     string percentPayedValue = installmentCalculatorSheet2.Range[$"AB{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(percentPayedValue, out decimal percentPayedValueD);
                                                     string pennaltiesPayedValue = installmentCalculatorSheet2.Range[$"AC{lastPamentRow}"].FormulaValue.ToString().Replace(".", ",");
                                                     decimal.TryParse(pennaltiesPayedValue, out decimal pennaltiesPayedValueD);
                                                     calculationResult.TaCredSumPayout = realPayedValueD + percentPayedValueD + pennaltiesPayedValueD;
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.TaCredSumPayout = lastPaymentFromBase.Amount;
                                                     break;
                                                 default:
                                                     break;
                                             }
                                         }

                                         // 4 Date_Of_Last_Payment
                                         if (lastPaymentFromBase != null)
                                         {
                                             switch (dataCounter.GetPeriod(lastPaymentFromBase.Date))
                                             {
                                                 case Periods.BeforeGrace:
                                                     //
                                                     break;
                                                 case Periods.InGrace:
                                                     calculationResult.DateOfLastPayment = lastPaymentFromBase.Date;
                                                     break;
                                                 case Periods.AfterGrace:
                                                     string realPayedValue = dataCounter.GetLastNotEmptyOrZeroValue("AA", currentDateRow);
                                                     int realPaymentRowIndex = dataCounter.GetRowWithNullValues("AA", realPayedValue);
                                                     string realPayedPercentValue = dataCounter.GetLastNotEmptyOrZeroValue("AB", currentDateRow);
                                                     int realPercentRowIndex = dataCounter.GetRowWithNullValues("AB", realPayedPercentValue);
                                                     int dateIndex = realPaymentRowIndex > realPercentRowIndex ? realPaymentRowIndex : realPercentRowIndex;

                                                     string realPaymentDate = installmentCalculatorSheet2.Range[$"H{dateIndex}"].FormulaValue.ToString();
                                                     DateTime.TryParse(realPaymentDate, out DateTime realPaymentDateD);
                                                     calculationResult.DateOfLastPayment = realPaymentDateD;
                                                     break;
                                                 case Periods.AfterContract:
                                                     calculationResult.DateOfLastPayment = lastPaymentFromBase.Date;
                                                     break;
                                                 default:
                                                     break;
                                             }

                                         }
                                        // calculationREsults.Add(calculationResult);
                                     }*/
                                    #endregion
                                    
                                    installmentCalculator2.Dispose();
                                    _logger.Information($"Для договора {item.ToString()} корректное окончание формирования калькулятора и внесения в базу отчетов");
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex, $"Для договора {item.ToString()} ОШИБКА формирования калькулятора и внесения в базу отчетов");
                                }
                                break;
                            case credit:
                                try
                                {
                                    var currentCultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
                                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
                                    string tempStr = _excelTemplaesOptions.PdlTemplatePath;
                                    Workbook pdlCalculator = new Workbook();
                                    pdlCalculator.LoadFromFile(_excelTemplaesOptions.PdlTemplatePath);
                                    Worksheet pdlCalculatorSheet = pdlCalculator.Worksheets[0];
                                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCultureInfo;

                                    _logger.Information($"Для договора {item.ToString()} старт формирования калькулятора и внесения в базу отчетов");
                                    DataCounter pdlDataCounter = new DataCounter(graceContract, pdlCalculatorSheet, _logger);
                                    pdlCalculatorSheet.SetCellValue(2, 2, graceContract.PrincipalPayable.ToString()); //Сумма
                                    pdlCalculatorSheet.SetCellValue(3, 2, graceContract.LoanRateDay.ToString("0.00", System.Globalization.CultureInfo.GetCultureInfo("ru-RU")) + "%"); //Ставка
                                    pdlCalculatorSheet.SetCellValue(4, 2, graceContract.LoanPeriodDaily.ToString()); //Срок

                                    pdlCalculatorSheet.Range["B5"].DateTimeValue = graceContract.ContractSignedOn.Date;//Дата заключения договора

                                    pdlCalculatorSheet.SetCellValue(8, 2, graceContract.PrincipalPzPayable.ToString());//Сумма ОД
                                    pdlCalculatorSheet.SetCellValue(9, 2, graceContract.TotalPercentCharge.ToString());//Начислено процентов
                                    pdlCalculatorSheet.SetCellValue(10, 2, graceContract.TotalPercentPzPayable.ToString());//Выплачено процентов
                                    pdlCalculatorSheet.SetCellValue(11, 2, graceContract.PenaltyPzPayable.ToString()); //Долг пени

                                    pdlCalculatorSheet.Range["B12"].DateTimeValue = graceContract.OldDateClosedContract.Date; // Текущая дата закрытия займа по договору

                                    pdlCalculatorSheet.Range["B17"].DateTimeValue = graceContract.GracePeriodStartDate;//Дата входа на КК
                                    pdlCalculatorSheet.Range["B18"].DateTimeValue = graceContract.GracePeriodEndDate;//Дата выхода с КК

                                    pdlCalculatorSheet.Range["B22"].DateTimeValue = DateTime.Now.Date;//Дата полного погашения

                                    //https://jira.bistrodengi.ru/browse/SMIS-4088
                                    pdlCalculatorSheet.SetCellValue(14, 2, graceContract.PaymentPenalty.ToString()); //Долг пени

                                    pdlCalculator.CalculateAllValue();
                                    string currentPdlPath = $"{_calculatorsSavePathOptions.Pdl}{graceContract.ContractNumber}.xlsx";
                                    pdlCalculator.SaveToFile(currentPdlPath);
                                    pdlCalculator.Dispose();
                                    //сохранили т.к при чтении глючит.

                                    //забираем Для записи оплат
                                    var currentCultureInfoPdl1 = System.Threading.Thread.CurrentThread.CurrentCulture;
                                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                                    Workbook pdlCalculator1 = new Workbook();
                                    pdlCalculator1.LoadFromFile(currentPdlPath);
                                    Worksheet pdlCalculatorSheet1 = pdlCalculator1.Worksheets[0];
                                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCultureInfoPdl1;

                                    DataCounter dataCounterForpayments = new DataCounter(graceContract, pdlCalculatorSheet1, _logger);

                                    foreach (var payment in payments)
                                    {
                                        int rowIndexFcolumn = dataCounterForpayments.GetRow("F", payment.Date.Date);
                                        if (rowIndexFcolumn > 0)
                                        {
                                            pdlCalculatorSheet1.SetCellValue(rowIndexFcolumn, 10, payment.Amount.ToString());
                                        }
                                        int rowIndexMcolumn = dataCounterForpayments.GetRow("M", payment.Date.Date);
                                        if (rowIndexMcolumn > 0)
                                        {
                                            pdlCalculatorSheet1.SetCellValue(rowIndexMcolumn, 19, payment.Amount.ToString());
                                        }
                                    }
                                    pdlCalculator1.CalculateAllValue();
                                    pdlCalculator1.SaveToFile(currentPdlPath);
                                    pdlCalculator1.Dispose();
                                    //запись оплат завершена

                                    //забираем данные Pdl
                                    var currentCultureInfoPdl = System.Threading.Thread.CurrentThread.CurrentCulture;
                                    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");

                                    Workbook pdlCalculator2 = new Workbook();
                                    pdlCalculator2.LoadFromFile(currentPdlPath);
                                    Worksheet pdlCalculatorSheet2 = pdlCalculator2.Worksheets[0];

                                    System.Threading.Thread.CurrentThread.CurrentCulture = currentCultureInfoPdl;
                                    DataCounter dataCounterpdlGeter = new DataCounter(graceContract, pdlCalculatorSheet2, _logger);


                                    _logger.Information($"старт наполнения Данных PDL SupplementaryAgreement для {graceContract.ContractNumber}");

                                    SupplementaryAgreement supplementaryAgreementPdl = new SupplementaryAgreement
                                    {
                                        Id = Guid.NewGuid(),
                                        ContractId = item,
                                        ContractNumberABS = graceContract.ContractNumberABS,
                                        GracePeriodStartDate = graceContract.GracePeriodStartDate
                                    };
                                    try
                                    {
                                        // _logger.LogInformation("Start try");
                                        string newContractPdlEndDateString = pdlCalculatorSheet2.Range[$"B13"].FormulaValue.ToString();
                                        DateTime.TryParse(newContractPdlEndDateString, out DateTime newpdlContractEndDate);
                                        supplementaryAgreementPdl.NewContractEndDate = newpdlContractEndDate;
                                        // supplementaryAgreements.Add(supplementaryAgreementPdl);

                                        // _logger.LogInformation("Удаляем старые SupplementaryAgreements");
                                        //Удаляем старые SupplementaryAgreements
                                        var supplementaryAgreementPdlToDelete = dbContext.SupplementaryAgreements.
                                            FirstOrDefault(x => x.ContractNumberABS == supplementaryAgreementPdl.ContractNumberABS);
                                        if (supplementaryAgreementPdlToDelete != null)
                                        {
                                            // _logger.LogInformation("supplementaryAgreementPdlToDelete != null");
                                            dbContext.SupplementaryAgreements.Remove(supplementaryAgreementPdlToDelete);
                                            //  _logger.LogInformation("supplementaryAgreementPdlToDelete != null SaveChanges()");
                                            dbContext.SaveChanges();
                                        }
                                        //Добавляем новые SupplementaryAgreements
                                        //   _logger.LogInformation("Добавляем новые SupplementaryAgreements");
                                        dbContext.SupplementaryAgreements.Add(supplementaryAgreementPdl);
                                        //  _logger.LogInformation("supplementaryAgreementPdlToDelete != null SaveChanges()");
                                        dbContext.SaveChanges();
                                    }
                                    catch (Exception ex)
                                    {
                                        _logger.Error(ex, $"Ошибка формирования PDL SupplementaryAgreements | {graceContract.ContractNumber} ошибка {ex.InnerException?.ToString()}");
                                    }
                                    _logger.Information($"старт удаления старых PDL данных InterestCharges для {graceContract.ContractNumber}");

                                    //Удаляем старые InterestCharges
                                    var interestChargesPdlTodelete = dbContext.InterestCharges.Where(x => x.ContractNumberABS == supplementaryAgreementPdl.ContractNumberABS);
                                    if (interestChargesPdlTodelete.Any())
                                    {
                                        foreach (var interestChargePdl in interestChargesPdlTodelete)
                                        {
                                            dbContext.Remove(interestChargePdl);
                                        }
                                        dbContext.SaveChanges();
                                    }

                                    _logger.Information($"старт создания PDL InterestCharges данных InterestCharges для {graceContract.ContractNumber}");

                                    Dictionary<YearAndMonth, decimal> pdlMonthlyPercentage = new Dictionary<YearAndMonth, decimal>();
                                    dataCounterpdlGeter.GetMonthSum("F", "H", ref pdlMonthlyPercentage);
                                    dataCounterpdlGeter.GetMonthSum("M", "O", ref pdlMonthlyPercentage);
                                    foreach (var sumAndDate in pdlMonthlyPercentage)
                                    {
                                        try
                                        {
                                            YearAndMonth yearAndMonth = sumAndDate.Key;
                                            DateTime dateTime = new DateTime(yearAndMonth.year, yearAndMonth.month,
                                                DateTime.DaysInMonth(yearAndMonth.year, yearAndMonth.month));

                                            decimal correctContractAmount = 0;
                                            if (sumAndDate.Value > 0)
                                            {
                                                correctContractAmount = sumAndDate.Value;
                                            }

                                            InterestCharge interestCharge = new InterestCharge
                                            {
                                                Id = Guid.NewGuid(),
                                                ContractId = item,
                                                ContractNumberABS = graceContract.ContractNumberABS,
                                                ContractType = 1,
                                                ContractKind = 118,
                                                ContractAmount = correctContractAmount,
                                                LastDayOfMonth = dateTime
                                            };
                                            //Добавляем новые InterestCharges
                                            dbContext.InterestCharges.Add(interestCharge);

                                            dbContext.SaveChanges();
                                        }
                                        catch (Exception ex)
                                        {
                                            _logger.Error(ex, $" Запись PDL InterestCharge ошибка {graceContract.ContractNumber}");
                                        }
                                        //interestCharges.Add(interestCharge);
                                    }

                                   
                                    pdlCalculator2.Dispose();
                                    _logger.Information($"Для договора {item.ToString()} корректное окончание формирования калькулятора и внесения в базу отчетов");
                                }
                                catch (Exception ex)
                                {
                                    _logger.Error(ex, $"Для договора {item.ToString()} ОШИБКА формирования калькулятора и внесения в базу отчетов");
                                }

                                break;
                            default:
                                _logger.Debug(" BankingServiceType {BankingServiceType} не предусмтренный тип!!", graceContract.BankingServiceType);
                                break;
                        }

                    }
                    //var contractsToclear = supplementaryAgreements.Select(x => x.ContractId).ToList();

                    //var interestChargesToDelete = dbContext.InterestCharges.Where(x => contractsToclear.Contains(x.ContractId));
                    //if (interestChargesToDelete.Any())
                    //{
                    //    dbContext.InterestCharges.RemoveRange(interestChargesToDelete);
                    //    var supplementaryAgreementsToDelete = dbContext.SupplementaryAgreements.Where(x => contractsToclear.Contains(x.ContractId));
                    //    dbContext.SupplementaryAgreements.RemoveRange(supplementaryAgreementsToDelete);
                    //    dbContext.SaveChanges();
                    //}
                    _logger.Information("завершение задачи");
                    //dbContext.InterestCharges.AddRange(interestCharges);
                    //dbContext.SupplementaryAgreements.AddRange(supplementaryAgreements);
                    ////dbContext.CalculationResults.AddRange(calculationREsults);
                    //dbContext.SaveChanges();
                }
                
                
            }
            catch (Exception ex)
            {
                _logger.Debug(ex.InnerException, ex.InnerException.ToString());
            }
        }

        private int CorrectRowsCountValue(int value, int curretDateRow)
        {
            if (value <= 3)
            {
                return curretDateRow - additionalRow;
            }
            return value;
        }

        private Logger CreateLogger(string path, LogEventLevel level)
        {
            if (File.Exists(path))
                File.Delete(path);

            string template =
                "{Timestamp:yyyy-MM-dd HH:mm:ss.fff}|{Level:u3}| {Message:lj}{NewLine}{Exception}";

            return new LoggerConfiguration()
                .WriteTo
                .File(path, level, template)
                .CreateLogger();
        }
    }
}
