using Microsoft.AspNetCore.Http;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Domain.Interfaces {
    public interface IBulkImportService {
        Task<BulkImportJob> CreateAsync(CancellationToken ct);
        Task<BulkImportJob?> GetJobByIdAsync(string id, CancellationToken ct);
        Task UpdateProcessingAsync(string id, CancellationToken ct);
        Task ProcessAsync(string id, string filePath, CancellationToken ct);
        Task CompleteAsync(string id, int total, int passed, string failedFileId, CancellationToken ct);
        Task FailAsync(string id, string reason, CancellationToken ct);
    }
}
