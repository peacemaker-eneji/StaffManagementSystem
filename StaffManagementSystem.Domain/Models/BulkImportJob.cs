using StaffManagementSystem.Domain.Enums;

namespace StaffManagementSystem.Domain.Models {

    public class BulkImportJob {
        public string Id { get; set; }
        public string FailedFileId { get; set; } = ""; // Id of the file of failed records
        public string ErrorMessage { get; set; } = "";
        public int Total { get; set; }
        public int Passed { get; set; }
        public int Failed { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public BulkImportJobStatus Status { get; set; } = BulkImportJobStatus.Queued;
    }
}
