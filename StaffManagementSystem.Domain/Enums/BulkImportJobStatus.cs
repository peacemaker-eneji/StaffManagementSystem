using System.ComponentModel;

namespace StaffManagementSystem.Domain.Enums {
    public enum BulkImportJobStatus {
        Queued,
        Processing,
        Failed,
        Completed,
        [Description("Completed with errors")]
        CompletedWithErrors
    }
}
