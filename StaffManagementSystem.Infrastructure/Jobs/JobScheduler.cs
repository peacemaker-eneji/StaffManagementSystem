using Hangfire;

namespace StaffManagementSystem.Infrastructure.Jobs {
    public static class JobScheduler {
        public static void RegisterRecurringJobs() {
            // runs every day at 9 PM
            RecurringJob.AddOrUpdate<AutoMarkAbsentJob>(
                "auto-mark-absent-attendance",
                job => job.RunAsync(),
                "0 21 * * *");
            RecurringJob.AddOrUpdate<AutoCheckOutJob>(
                "auto-checkout-attendance",
                job => job.RunAsync(),
                "0 21 * * *"); 

            // every 6hrs
            RecurringJob.AddOrUpdate<RefreshCalendarCacheJob>(
                "refresh-calendar-cache",
                job => job.RunAsync(),
                Cron.HourInterval(6));

            // trigger on restarts
            using (var connection = JobStorage.Current.GetConnection()) {
                var manager = new RecurringJobManager(JobStorage.Current);
                manager.TriggerJob("refresh-calendar-cache");
            }


        }
    }
}
