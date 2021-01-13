using DPSService.Options;
using Quartz.Spi;
using System.Threading.Tasks;

namespace DPSService.Quartz
{
    public interface IApplicationScheduler
    {
        Task UseQuartz(ScheduleOptions scheduleOptions);
    }
}