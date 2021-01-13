using DPSService.Jobs;
using DPSService.Options;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Quartz.Impl;
using Quartz.Spi;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;

namespace DPSService.Quartz
{
//    public class ApplicationScheduler : IApplicationScheduler
//    {
//        IServiceProvider _serviceprovider;
//        public ApplicationScheduler(IServiceProvider serviceprovider)
//        {
//            _serviceprovider = serviceprovider;
//        }
//        public const string name = "DPSService";
//        public async Task UseQuartz(ScheduleOptions scheduleOptions)
//        {
//            var properties = new NameValueCollection { { "quartz.threadPool.threadCount", "2" } };

//            ISchedulerFactory schedulerFactory = new StdSchedulerFactory(properties);

//            var scheduler = await schedulerFactory.GetScheduler().ConfigureAwait(false);
//            scheduler.JobFactory = new JobFactory(_serviceprovider);
//            await scheduler.Start().ConfigureAwait(false);
//            var voteJob = JobBuilder.Create<CreateReportJob>()
//.WithIdentity("myJob", "group1") // name "myJob", group "group1"
// .Build();

//            var temp = DateTime.Now.Hour;
//            var temp2 = DateTime.Now.Minute;

//            var voteJobTrigger = TriggerBuilder.Create()
//              .WithIdentity(name)
//              .WithSchedule(CronScheduleBuilder
//              .DailyAtHourAndMinute(temp, temp2 + 1))
//              // .DailyAtHourAndMinute(scheduleOptions.StartHour, scheduleOptions.StartMinute))
//              // .WithSimpleSchedule(x => x.WithIntervalInMinutes(scheduleOptions.RepeatRate).WithRepeatCount(scheduleOptions.RepeatCount))
//              .Build();

//            await scheduler.ScheduleJob(voteJob, voteJobTrigger).ConfigureAwait(false);
//        }
//    }
}
