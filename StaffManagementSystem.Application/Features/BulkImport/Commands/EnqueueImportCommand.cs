using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Application.Features.BulkImport.Commands {
    public record EnqueueImportResponse(string JobId);
    public record EnqueueImportCommand(IFormFile file) : IRequest<ApiResponse<EnqueueImportResponse>>;

    public class EnqueueImportCommandHandler : IRequestHandler<EnqueueImportCommand, ApiResponse<EnqueueImportResponse>> {
        private readonly IBulkImportService _bulkImportService;
        private readonly IWebHostEnvironment _env;

        public EnqueueImportCommandHandler(IBulkImportService bulkImportService, IWebHostEnvironment env) {
            _bulkImportService = bulkImportService;
            _env = env;
        }

        public async Task<ApiResponse<EnqueueImportResponse>> Handle(EnqueueImportCommand request, CancellationToken ct) {
            var importJob = await _bulkImportService.CreateAsync(ct);

            var filePath = Path.Combine(_env.WebRootPath, "Storage", "BulkImports", "Uploads", request.file.FileName);
            await using var stream = File.Create(filePath);
            await request.file.CopyToAsync(stream);

            BackgroundJob.Enqueue(() => _bulkImportService.ProcessAsync(importJob.Id, filePath, ct));

            return new ApiResponse<EnqueueImportResponse> {
                Status = StatusCodes.Status202Accepted,
                Message = "Bulk Import queued successfully",
                Data = new EnqueueImportResponse(importJob.Id)
            };
        }
    }
}

