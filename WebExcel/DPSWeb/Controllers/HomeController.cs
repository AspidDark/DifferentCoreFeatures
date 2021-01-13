using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using DPSWeb.Models;
using DPSWeb.Reports;

namespace DPSWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISupplementaryAgreementReport _supplementaryAgreementReport;
        private readonly IInterestChargeReport _interestChargeReport;
        public const string additionalContracts = "РЕЕСТР ДОП. СОГЛАШЕНИЙ";
        public const string percent = "РЕЕСТР НАЧИСЛЕННЫХ %";
        public HomeController(ILogger<HomeController> logger,
            ISupplementaryAgreementReport supplementaryAgreementReport,
            IInterestChargeReport interestChargeReport)
        {
            _logger = logger;
            _supplementaryAgreementReport = supplementaryAgreementReport;
            _interestChargeReport = interestChargeReport;
        }
        [HttpGet]
        public IActionResult Index()
        {
            ReportModel reportModel = new ReportModel();
            reportModel.title = additionalContracts;
            return View(reportModel);
        }

        [HttpPost]
        public async Task<IActionResult> Index(int month, int year)
        {
            _logger.LogInformation($"Запрос на создание {additionalContracts}");
            year = year <= 2019 ? 2020 : year;
            try
            {
                var fileModel = _supplementaryAgreementReport.GetReport(month, year);
                _logger.LogInformation($"Запрос на создание {additionalContracts} успешен отдан файл {fileModel.Name}");
                return File(fileModel.Path, "application/octet-stream", fileModel.Name);
            }
            catch(Exception e)
            {
                _logger.LogError(e, $"Ошибка формирования {additionalContracts}");
                return BadRequest();
            }
        }
        [HttpGet]
        public IActionResult Privacy()
        {
            ReportModel reportModel = new ReportModel();
            reportModel.title = percent;
            return View(reportModel);
        }
        [HttpPost]
        public async Task<IActionResult> Privacy(int month, int year)
        {
            _logger.LogInformation($"Запрос на создание {percent}");
            year = year <= 2019 ? 2020 : year;
            try
            {
                var fileModel = _interestChargeReport.GetReport(month, year);
                _logger.LogInformation($"Запрос на создание {percent} успешен отдан файл {fileModel.Name}");
                return File(fileModel.Path, "application/octet-stream", fileModel.Name);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Ошибка формирования {percent}");
                return BadRequest();
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
