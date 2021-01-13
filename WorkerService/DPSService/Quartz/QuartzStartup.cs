using DPSService.Jobs;
using DPSService.Options;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.Text;

namespace DPSService.Quartz
{
    public class QuartzStartup
    {
        //private IScheduler _scheduler; // after Start, and until shutdown completes, references the scheduler object
        //private readonly IServiceProvider container;

        //public IConfiguration Configuration { get; }

        //public const string name = "DPSService";
        //private readonly ILogger<Worker> _logger;
        //private readonly ScheduleOptions scheduleOptions;

        //public QuartzStartup(IServiceProvider container, IConfiguration configuration, ILogger<Worker> logger)
        //{
        //    Configuration = configuration;
        //    _logger = logger;
        //    scheduleOptions = new ScheduleOptions();
        //    Configuration.GetSection(nameof(ScheduleOptions)).Bind(scheduleOptions);
        //    this.container = container;
        //}

        //// starts the scheduler, defines the jobs and the triggers
        //public void Start()
        //{
        //    if (_scheduler != null)
        //    {
        //        throw new InvalidOperationException("Already started.");
        //    }

        //    var schedulerFactory = new StdSchedulerFactory();
        //    _scheduler = schedulerFactory.GetScheduler().Result;
        //    _scheduler.JobFactory = new JobFactory(container);
        //    _scheduler.Start().Wait();


        //    var voteJob = JobBuilder.Create<CreateReportJob>()
        //    .WithIdentity("myJob", "group1") // name "myJob", group "group1"
        //     .Build();

        //    var temp = DateTime.Now.Hour;
        //    var temp2 = DateTime.Now.Minute;

        //    var voteJobTrigger = TriggerBuilder.Create()
        //      .WithIdentity(name)
        //      .WithSchedule(CronScheduleBuilder
        //      .DailyAtHourAndMinute(temp, temp2 + 2))
        //      // .DailyAtHourAndMinute(scheduleOptions.StartHour, scheduleOptions.StartMinute))
        //      // .WithSimpleSchedule(x => x.WithIntervalInMinutes(scheduleOptions.RepeatRate).WithRepeatCount(scheduleOptions.RepeatCount))
        //      .Build();

        //    _scheduler.ScheduleJob(voteJob, voteJobTrigger).Wait();
        //}

        //// initiates shutdown of the scheduler, and waits until jobs exit gracefully (within allotted timeout)
        //public void Stop()
        //{
        //    if (_scheduler == null)
        //    {
        //        return;
        //    }

        //    // give running jobs 30 sec (for example) to stop gracefully
        //    if (_scheduler.Shutdown(waitForJobsToComplete: true).Wait(30000))
        //    {
        //        _scheduler = null;
        //    }
        //    else
        //    {
        //        // jobs didn't exit in timely fashion - log a warning...
        //    }
        //}
    }
}
