using ClosedXML.Excel;
using DPSWeb.Data;
using DPSWeb.Models;
using DPSWeb.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DPSWeb.Reports
{
    public class InterestChargeReport : IInterestChargeReport
    {
        public const string report = "Отчет";
        private readonly IInterestChargeService _interestChargeService;
        private readonly ILogger<InterestChargeReport> _logger;
        public InterestChargeReport(IInterestChargeService interestChargeService, ILogger<InterestChargeReport> logger)
        {
            _interestChargeService = interestChargeService;
            _logger = logger;
        }

        public FileModel GetReport(int month, int yerar)
        {
            List<InterestCharge> interestCharges = _interestChargeService.GetByDate(month, yerar);

            string fileName = $"ICReport{DateTime.Now.ToString("yyyy_MM_dd_hh_mm_ss")}.xlsx";
            _logger.LogInformation($"Для файла {fileName} найдено {interestCharges.Count} договоров");
            //var toLog = interestCharges.Select(x=>x.ContractNumberABS).Aggregate((z, y)=>z +" " +y);
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Uploads", fileName);
            var workbook = new XLWorkbook();
            workbook.AddWorksheet(report);
            var ws = workbook.Worksheet(report);
            //Заголовок
            ws.Row(1).Cell(1).SetValue<string>("Код договора");
            ws.Row(1).Cell(2).SetValue<string>("Тип");
            ws.Row(1).Cell(3).SetValue<string>("Вид");
            ws.Row(1).Cell(4).SetValue<string>("Сумма");
            ws.Row(1).Cell(5).SetValue<string>("Дата");
            for (int i = 0; i < interestCharges.Count; i++)
            {
                ws.Row(i + 2).Cell(1).SetValue<string>(interestCharges[i].ContractNumberABS);
                ws.Row(i + 2).Cell(2).SetValue<string>(interestCharges[i].ContractType.ToString());
                ws.Row(i + 2).Cell(3).SetValue<string>(interestCharges[i].ContractKind.ToString());
                ws.Row(i + 2).Cell(4).SetValue<string>(interestCharges[i].ContractAmount.ToString());

                ws.Row(i + 2).Cell(5).SetValue<DateTime>(interestCharges[i].LastDayOfMonth);
                // ws.Row(i + 2).Cell(5).SetValue<string>(interestCharges[i].LastDayOfMonth.ToString("dd/MM/yyyy").Replace('/','.'));
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
