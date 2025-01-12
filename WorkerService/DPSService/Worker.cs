using System;
using System.Threading;
using System.Threading.Tasks;
using DPSService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DPSService.Quartz;
using Microsoft.Extensions.DependencyInjection;
using DPSService.Jobs;
using DPSService.DB;
using System.Linq;

namespace DPSService
{
    //https://stackoverflow.com/questions/59727799/how-to-run-net-core-ihosted-service-at-specific-time-of-the-day
    public class Worker : BackgroundService
    {
        public const string name = "DPSService";
        private readonly ILogger<Worker> _logger;
        private readonly CreateReportJob _createReport;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ScheduleOptions _scheduleOptions;
        //отсюда тащтить скоуп
        private readonly IServiseScopeFactory _serviseScopeFactory;
        //Доступ к лайфтайму всей приложухи
        private readonly IHostApplicationLifetime _hostApplicationLifetime;

        public IConfiguration Configuration { get; }
        public Worker(IConfiguration configuration, ILogger<Worker> logger, CreateReportJob createReport, IServiceScopeFactory scopeFactory)
        {
            Configuration = configuration;
            _scheduleOptions = new ScheduleOptions();
            Configuration.GetSection(nameof(ScheduleOptions)).Bind(_scheduleOptions);
            _scopeFactory = scopeFactory;
            _logger = logger;
            _createReport = createReport;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<DataContext>();
                    var integrationEvent = dbContext.IntegrationEvents.FirstOrDefault(x => x.ItegarionDate.Date == DateTime.Now.Date);
                    if (integrationEvent != null && integrationEvent.Complete == 0)
                    {
                        await _createReport.Execute();
                        integrationEvent.Complete = 1;
                        dbContext.SaveChanges();
                    }
                }
                //спим час
                _logger.LogInformation("Worker running at: {time}", DateTimeOffset.Now);
                await Task.Delay(_scheduleOptions.SleepDelay, stoppingToken);
            }
            _logger.LogWarning("Worker stoped at: {time}", DateTimeOffset.Now);
        }
    }
}
