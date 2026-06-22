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
        }
    }
}
