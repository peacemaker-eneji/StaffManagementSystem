using DotNetEnv;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using StaffManagementSystem.Application.Features.BulkImport.Commands;
using StaffManagementSystem.Application.Features.BulkImport.Queries;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Models;

namespace StaffManagementSystem.Api.Controllers {

    //[Authorize(Roles = nameof(Role.Admin))]
    [ApiController]
    [Route("bulk-import")]
    public class ImportController : ControllerBase {
        private readonly ISender _sender;
        private readonly IWebHostEnvironment _env;

        public ImportController(ISender sender, IWebHostEnvironment env) {
            _sender = sender;
            _env = env;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<EnqueueImportResponse>>> Import(IFormFile file) {
            var response = await _sender.Send(new EnqueueImportCommand(file));
            return StatusCode(response.Status, response);
        }

        [HttpGet("{jobId}")]
        public async Task<IActionResult> GetStatus(string jobId) {
            var response = await _sender.Send(new GetImportStatusQuery(jobId));
            return StatusCode(response.Status, response);
        }

        [HttpGet("failed-imports/{id}/{type}")]
        public async Task<ActionResult> GetFailedImportTemplate(string id, string type) {
            if (type != "csv" && type != "xlsx") return BadRequest("Invalid file type");

            var filePath = Path.Combine(_env.WebRootPath, "Storage", "BulkImports", "FailedImports", $"{id}.{type}");
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", $"{id}.{type}");
        }

        [HttpGet("templates/{type}")]
        public async Task<ActionResult> GetBulkImportTemplate(string type) {
            if (type != "csv" && type != "xlsx") return BadRequest("Invalid template type");

            var fileName = "BulkUserImportTemplate";
            var filePath = Path.Combine(_env.WebRootPath, "Storage", "Templates", $"{fileName}.{type}");
            if (!System.IO.File.Exists(filePath)) return NotFound();

            var bytes = System.IO.File.ReadAllBytes(filePath);
            return File(bytes, "application/octet-stream", $"{fileName}.{type}");
        }
    }
}
