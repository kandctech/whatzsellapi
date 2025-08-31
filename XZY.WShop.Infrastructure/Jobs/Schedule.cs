using Coravel;

namespace XZY.WShop.Infrastructure.Jobs
{
    public class Schedule
    {
        public static void ScheduleJobs(IServiceProvider provider)
        {
            provider.UseScheduler(scheduler =>
            {

                scheduler.Schedule<FollowUpReminderJob>()
              .EveryMinute()
              .PreventOverlapping("FollowUpReminderJob");

            });
        }
    }
}
