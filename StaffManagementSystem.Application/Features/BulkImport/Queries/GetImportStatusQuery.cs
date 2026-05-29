using MediatR;
using Microsoft.AspNetCore.Http;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using Superpower.Model;

namespace StaffManagementSystem.Application.Features.BulkImport.Queries {
    public record GetImportStatusQuery(string JobId) : IRequest<ApiResponse<BulkImportJob>>;

    public class GetImportStatusHandler : IRequestHandler<GetImportStatusQuery, ApiResponse<BulkImportJob>> {
        private readonly IBulkImportService _bulkImportService;

        public GetImportStatusHandler(IBulkImportService bulkImportService) {
            _bulkImportService = bulkImportService;
        }

        public async Task<ApiResponse<BulkImportJob>> Handle(GetImportStatusQuery request, CancellationToken ct) {
            var importJob = await _bulkImportService.GetJobByIdAsync(request.JobId, ct);

            if (importJob is null) {
                return new ApiResponse<BulkImportJob> {
                    Status = StatusCodes.Status404NotFound,
                    Message = "Bulk Import Job not found",
                    Success = false
                };
            }
            int statusCode = importJob.Status switch {
                BulkImportJobStatus.Completed => StatusCodes.Status200OK,
                BulkImportJobStatus.CompletedWithErrors => StatusCodes.Status200OK,
                BulkImportJobStatus.Failed => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status202Accepted
            };

            return new ApiResponse<BulkImportJob> {
                Status = statusCode,
                Message = "Fetched Import Job successfully",
                Data = importJob,
                Success = importJob.Status != BulkImportJobStatus.Failed
            };
        }
    }
}
