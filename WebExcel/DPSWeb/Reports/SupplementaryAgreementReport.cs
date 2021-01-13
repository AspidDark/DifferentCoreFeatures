using DPSWeb.Data;
using DPSWeb.Services;
using ClosedXML.Excel;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using DPSWeb.Models;

namespace DPSWeb.Reports
{
    public class SupplementaryAgreementReport : ISupplementaryAgreementReport
    {
        public const string report = "Отчет";
        private readonly ISupplementaryAgreementService _supplementaryAgreementService;
        private readonly ILogger<SupplementaryAgreementReport> _logger;
        public SupplementaryAgreementReport(ISupplementaryAgreementService supplementaryAgreementService,
            ILogger<SupplementaryAgreementReport> logger)
        {
            _supplementaryAgreementService = supplementaryAgreementService;
            _logger = logger;
        }
        public FileModel GetReport(int month, int year)
        {
            List<SupplementaryAgreement> supplementaryAgreements = _supplementaryAgreementService
                .GetByDate(month, year);
            string fileName = $"SAReport{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.xlsx";
            _logger.LogInformation($"Для файла {fileName} найдено {supplementaryAgreements.Count} договоров");
            //var toLog = interestCharges.Select(x=>x.ContractNumberABS).Aggregate((z, y)=>z +" " +y);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads", fileName);
            var workbook = new XLWorkbook();
            workbook.AddWorksheet(report);
            var ws = workbook.Worksheet(report);
            //Заголовок
            ws.Row(1).Cell(1).SetValue<string>("Идентификатор займа");
            ws.Row(1).Cell(2).SetValue<string>("Дата начала кредитных каникул");
            ws.Row(1).Cell(3).SetValue<string>("Новая дата окончания займа");
            for (int i = 0; i < supplementaryAgreements.Count; i++)
            {
                ws.Row(i+2).Cell(1).SetValue<string>(supplementaryAgreements[i].ContractNumberABS);
              //  ws.Row(i + 2).Cell(2).SetDataType(XLDataType.DateTime);
                ws.Row(i + 2).Cell(2).SetValue<DateTime>(supplementaryAgreements[i].GracePeriodStartDate);
             //   ws.Row(i + 2).Cell(3).SetDataType(XLDataType.DateTime);
                ws.Row(i + 2).Cell(3).SetValue<DateTime>(supplementaryAgreements[i].NewContractEndDate);
                //ws.Row(i+2).Cell(2).SetValue<string>(supplementaryAgreements[i].GraceEndDate.ToString("dd/MM/yyyy").Replace('/', '.'));
                //ws.Row(i+2).Cell(3).SetValue<string>(supplementaryAgreements[i].NewContractEndDate.ToString("dd/MM/yyyy").Replace('/', '.'));
            }
            workbook.SaveAs(filePath);
            workbook.Dispose();
            return new FileModel
            {
                Name = fileName,
                Path = @"\Uploads\" + fileName
            };
        }
    }
}
