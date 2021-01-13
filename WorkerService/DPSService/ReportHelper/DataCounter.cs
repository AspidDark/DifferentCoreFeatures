using DPSService.DB;
using DPSService.Jobs;
using Microsoft.Extensions.Logging;
using Serilog.Core;
using Spire.Xls;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace DPSService.ReportHelper
{
    
    public class DataCounter
    {
        public const string q1 = "2020Q1";
        public const string q2 = "2020Q2";
        public const string q3 = "2020Q3";
        public const string q4 = "2020Q4";
        private GraceContact _graceContact;
        private Worksheet _worksheet;
        private readonly Logger _logger;
        public DataCounter(GraceContact graceContract, Worksheet worksheet,
            Logger logger)
        {
            _graceContact = graceContract;
            _worksheet = worksheet;
            _logger = logger;
        }


        public void GetMonthSum(string firstColumnSerch, string firstColumnTuSum,
            ref Dictionary<YearAndMonth, decimal> monthlySums)
        {
            for (int i = 3; i < 200; i++)
            {
                var valueFromColumnObject = _worksheet.Range[$"{firstColumnSerch}{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                if (valueFromColumnObject == null)
                {
                    continue;
                }
                if (DateTime.TryParse(valueFromColumnObject.ToString(), out DateTime date))
                {
                    var valueFromH = _worksheet.Range[$"{firstColumnTuSum}{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                    string valueFromColumn = "00.00";
                    if (valueFromH != null)
                    {
                       // _logger.LogInformation($"--{i}-- {valueFromH.ToString()}");
                        valueFromColumn = valueFromH.ToString()?.Replace(".", ",");
                    }
                    decimal.TryParse(DataCounter.ShortString(valueFromColumn), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                    YearAndMonth yearAndMonth = new YearAndMonth { year = date.Year, month = date.Month };
                  //  _logger.LogInformation($"--{i}-- element--- {element}");
                    if (monthlySums.ContainsKey(yearAndMonth))
                    {
                        monthlySums[yearAndMonth] += element;
                    }
                    else
                    {
                        monthlySums.Add(yearAndMonth, element);
                    }
                }

            }
            //_logger.LogInformation($"------------------------------------------------------------------");
            //foreach (var item in monthlySums)
            //{
            //    _logger.LogInformation($"-year-{item.Key.year} -month-{item.Key.month}- Value--- {item.Value}");
            //}
        }

        public Dictionary<YearAndMonth, decimal> GetMonthSum()
        {
            Dictionary<YearAndMonth, decimal> monthlySums = new Dictionary<YearAndMonth, decimal>();
            for (int i = 3; i < 200; i++)
            {
                var valueFromColumnObject = _worksheet.Range[$"H{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                if (valueFromColumnObject == null)
                {
                    continue;
                }
                if (DateTime.TryParse(valueFromColumnObject.ToString(), out DateTime date))
                {
                    var valueFromH = _worksheet.Range[$"O{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                    string valueFromColumn = "00.00";
                    if (valueFromH != null)
                    {
                        valueFromColumn = valueFromH.ToString()?.Replace(".", ",");
                    }
                    decimal.TryParse(DataCounter.ShortString(valueFromColumn), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                    YearAndMonth yearAndMonth = new YearAndMonth { year = date.Year, month = date.Month };
                    if (monthlySums.ContainsKey(yearAndMonth))
                    {
                        monthlySums[yearAndMonth] += element;
                    }
                    else
                    {
                        monthlySums.Add(yearAndMonth, element);
                    }
                }
                 
            }
            for (int i = 3; i < 200; i++)
            {
                var valueFromColumnObject = _worksheet.Range[$"AE{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                if (valueFromColumnObject == null)
                {
                    continue;
                }
                if (DateTime.TryParse(valueFromColumnObject.ToString(), out DateTime date))
                {
                    var valueFromH = _worksheet.Range[$"AG{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                    string valueFromColumn = "00.00";
                    if (valueFromH != null)
                    {
                        valueFromColumn = valueFromH.ToString()?.Replace(".", ",");
                    }
                    decimal.TryParse(DataCounter.ShortString(valueFromColumn), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                    YearAndMonth yearAndMonth = new YearAndMonth { year = date.Year, month = date.Month };
                    if (monthlySums.ContainsKey(yearAndMonth))
                    {
                        monthlySums[yearAndMonth] += element;
                    }
                    else
                    {
                        monthlySums.Add(yearAndMonth, element);
                    }
                }

            }
            return monthlySums;
        }


        public string GetQuarter(DateTime dateTime) => dateTime.Month switch
        {
            1 => q1,
            2 => q1,
            3 => q1,
            4 => q2,
            5 => q2,
            6 => q2,
            7 => q3,
            8 => q3,
            9 => q3,
            _ => q4
        };

        public int GetRow(string column, string searchKey, int maxValue = 200)
        {
            for (int i = 3; i < maxValue; i++)
            {
                string columnValue = _worksheet.Range[$"{column}{i}"].FormulaValue.ToString();
                if (searchKey == columnValue)
                {
                    return i;
                }
            }
            return -1;
        }



        public int GetRow(string column, DateTime searchKey, int maxValue = 200)
        {
            for (int i = 3; i < maxValue; i++)
            {
                var columnValueString = _worksheet.Range[$"{column}{i}"].FormulaValue.ToString();
                DateTime.TryParse(columnValueString, out DateTime columnValue);
                if (searchKey == columnValue.Date)
                {
                    return i;
                }
            }
            return -1;
        }

        public int GetRowWithNullValues(string column, string searchKey, int maxValue = 200)
        {
            for (int i = 3; i < maxValue; i++)
            {
                object columnValueObject = _worksheet.Range[$"{column}{i}"].FormulaValue;
                if (columnValueObject!=null&& searchKey == columnValueObject.ToString())
                {
                    return i;
                }
            }
            return -1;
        }


        public int GetNextNotEmptyRowIndex(string column, int startValue,  int maxValue = 200)
        {
            for (int i = startValue; i < maxValue; i++)
            {
                string valueFromColumn = _worksheet.Range[$"{column}{i}"].FormulaValue.ToString();
                if (!string.IsNullOrWhiteSpace(valueFromColumn)&& valueFromColumn!="0")
                {
                    return i;
                }
            }
            return -1;
        }
        ///Rows Up Count
        public int RowsUpCount(string templateValue, string columnValue, int startIndex)
        {
            for (int i = startIndex - 1; i > 3; i--)
            {
                string valueFromColumn = _worksheet.Range[$"{columnValue}{i}"].FormulaValue.ToString();
                if (templateValue != valueFromColumn)
                {
                    return i;
                }
            }
            return -1;
        }
        ///Rows Up Count
        public int RowsUpCountEmptyValueInColumns(string columnValue, int startIndex)
        {
            for (int i = startIndex - 1; i > 3; i--)
            {
                string valueFromColumn = _worksheet.Range[$"{columnValue}{i}"].FormulaValue.ToString();
                if (!string.IsNullOrWhiteSpace(valueFromColumn)&& valueFromColumn!="0")
                {
                    return i;
                }
            }
            return 200;
        }

        //SummColumnValues
        public decimal SummColumnValues(string columnValue, int startIndex, int iterations)
        {
            decimal result = 0;
            for (int i = startIndex; i < startIndex + iterations; i++)
            {
                string valueFromColumn = _worksheet.Range[$"{columnValue}{i}"].FormulaValue.ToString()?.Replace(".", ",");
                decimal.TryParse(DataCounter.ShortString(valueFromColumn), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                result += element;
            }
            return result;
        }

        //SummColumnValues
        public decimal SummColumnValuesWithEmptyRows(string columnValue, int startIndex, int iterations)
        {
            decimal result = 0;
            for (int i = startIndex; i < startIndex + iterations; i++)
            {
                var valueFromColumnObject = _worksheet.Range[$"{columnValue}{i}"].FormulaValue;//.ToString()?.Replace(".", ",");
                string valueFromColumn = "00.00";
                if (valueFromColumnObject != null)
                {
                    valueFromColumn = valueFromColumnObject.ToString()?.Replace(".", ",");
                }
                decimal.TryParse(DataCounter.ShortString(valueFromColumn), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                result += element;
            }
            return result;
        }
        public decimal GetNextNotKeyValue(string key, int startIndex, int endIndex = 200)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                decimal.TryParse(DataCounter.ShortString(_worksheet.Range[$"{key}{i}"].FormulaValue.ToString().Replace(".", ",")), NumberStyles.Any, new CultureInfo("ru-RU"), out decimal element);
                if (element > 0)
                {
                    return element;
                }
            }
            return 0;
        }

        public bool IsInInterval(int value, int lowerBounds, int upperBounds)
        {
            return value >= lowerBounds && value <= upperBounds;
        }

        public string GetLastNotEmptyValue(string columnKey, int startIndex = 200)
        {
            for (int i = startIndex; i > 3; i--)
            {
                string columnValue = _worksheet.Range[$"{columnKey}{i}"].FormulaValue.ToString();
                if (!string.IsNullOrWhiteSpace(columnValue))
                {
                    return columnValue;
                }
            }
            return String.Empty;
        }

        public string GetLastNotEmptyOrZeroValue(string columnKey, int startIndex = 200)
        {
            for (int i = startIndex; i > 3; i--)
            {
                var columnValueObject = _worksheet.Range[$"{columnKey}{i}"].FormulaValue;
                if(columnValueObject!=null 
                    && !string.IsNullOrWhiteSpace(columnValueObject.ToString()) && columnValueObject.ToString()!="0")
                {
                    return columnValueObject.ToString();
                }
            }
            return String.Empty;
        }

        public string NextNotNullValue(string columnKey, int startIndex)
        {
            for (int i = startIndex; i < 200; i++)
            {
                string columnValue = _worksheet.Range[$"{columnKey}{i}"].FormulaValue.ToString();
                if (!string.IsNullOrWhiteSpace(columnValue)&& columnValue!="0")
                {
                    return columnValue;
                }
            }
            return String.Empty;
        }
        /// <summary>
        /// Дата окончания основного договора
        /// </summary>
        /// <returns></returns>
        public DateTime GetLoanEndDate()
        {
            int graceContractDurationInDays = (int)(_graceContact.GracePeriodEndDate - _graceContact.GracePeriodStartDate).TotalDays;
            return _graceContact.ContractSignedOn.AddDays(_graceContact.LoanPeriodDaily).
                AddDays(graceContractDurationInDays);
        }




        //Оплата до кредитных каникул
        public bool BeforeGracePeriod(DateTime dateTime)
        {
            return (_graceContact.ContractSignedOn <= dateTime) && (dateTime < _graceContact.GracePeriodStartDate);
        }
        //оплата после кредитных каникул Но !! не процентов
        public bool AfterGracePeriod(DateTime dateTime)
        {
            int graceContractDurationInDays = (int)(_graceContact.GracePeriodEndDate - _graceContact.GracePeriodStartDate).TotalDays;
            return (dateTime > _graceContact.GracePeriodEndDate)
                && (dateTime <= _graceContact.ContractSignedOn.AddDays(_graceContact.LoanPeriodDaily).
                AddDays(graceContractDurationInDays));
        }
        //оплата процентов по завершении договора
        public bool AfterContractPeriod(DateTime dateTime)
        {
            int graceContractDurationInDays = (int)(_graceContact.GracePeriodEndDate - _graceContact.GracePeriodStartDate).TotalDays;
            return (dateTime > _graceContact.ContractSignedOn.AddDays(_graceContact.LoanPeriodDaily).
                AddDays(graceContractDurationInDays));
        }


        //оплата во время кредитных каникул
        public bool PaymentDueGraceContractPeriod(DateTime paymentDate)
        {
            return (paymentDate >= _graceContact.GracePeriodStartDate) && (paymentDate <= _graceContact.GracePeriodEndDate);
        }

        public Periods GetPeriod(DateTime dateTime)
        {
            if (BeforeGracePeriod(dateTime))
            {
                return Periods.BeforeGrace;
            }
            if (PaymentDueGraceContractPeriod(dateTime))
            {
                return Periods.InGrace;
            }
            if (AfterGracePeriod(dateTime))
            {
                return Periods.AfterGrace;
            }
            return Periods.AfterContract;
        }

        public int GraceContractAndPaymentDaysEqual()
        {
            int graceContractStartInDays = (int)(_graceContact.GracePeriodStartDate - _graceContact.ContractSignedOn).TotalDays;
            if (graceContractStartInDays % _graceContact.TermVariance == 0)
            {
                return 1;
            }
            else
            {
                return 0;
            }
        }

        public int GraceContractAndPaymentDaysEqual2() =>
            (int)(_graceContact.GracePeriodStartDate - _graceContact.ContractSignedOn)
            .TotalDays % _graceContact.TermVariance == 0 ? 1 : 0;

        public static string ShortString(string incomeing)
        {
            if (incomeing.Length > 10)
            {
                return incomeing.Substring(0, 10);
            }
            return incomeing;
        }
    }
}
