using CsvHelper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using MiniExcelLibs;
using StaffManagementSystem.Domain.Enums;
using StaffManagementSystem.Domain.Interfaces;
using StaffManagementSystem.Domain.Models;
using StaffManagementSystem.Infrastructure.Persistence;
using System.Data;
using System.Globalization;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;





namespace StaffManagementSystem.Infrastructure.Services {

    public class UserData {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
    }

    public class UserDataFailed {
        public string Firstname { get; set; }
        public string Lastname { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public string Errors { get; set; } = "";
    }

    public class BulkImportService : IBulkImportService {
        private static int batchSize = 500;

        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _env;
        private readonly PasswordHasher<User> _passwordHasher = new PasswordHasher<User>();
        private readonly IConfiguration _configuration;

        private readonly DataTable _usersDataTable = new DataTable();
        private readonly DataTable _rolesDataTable = new DataTable();

        public BulkImportService(AppDbContext context, IWebHostEnvironment env, IConfiguration configuration) {
            _context = context;
            _env = env;
            _configuration = configuration;

            InitDataTables();
        }

        public async Task<BulkImportJob> CreateAsync(CancellationToken ct = default) {
            var job = new BulkImportJob {
                Id = Guid.NewGuid().ToString(),
                StartedAt = DateTime.Now
            };
            await _context.BulkImportJobs.AddAsync(job, ct);
            await _context.SaveChangesAsync(ct);
            return job;
        }

        public async Task<BulkImportJob?> GetJobByIdAsync(string jobId, CancellationToken ct = default) {
            return await _context.BulkImportJobs.FindAsync(jobId, ct);
        }

        public async Task UpdateProcessingAsync(string jobId, CancellationToken ct = default) {
            var job = await GetJobByIdAsync(jobId, ct);
            if (job is null) return;
            job.Status = BulkImportJobStatus.Processing;
            await _context.SaveChangesAsync(ct);
        }

        public async Task ProcessAsync(string jobId, string filePath, CancellationToken ct = default) {
            try {
                await UpdateProcessingAsync(jobId, ct);

                List<UserData> userDatas = await ReadUserDatas(filePath);
                List<UserDataFailed> failedImports = [];

                if (userDatas.Count==0) {
                    await FailAsync(jobId, "Unable to read file records", ct);
                    return;
                }


                var userDatasEmails = userDatas.Select(r => r.Email).ToHashSet();
                var existingEmails = await _context.Users
                    .Where(u => userDatasEmails.Contains(u.Email!))
                    .Select(u => u.Email)
                    .ToHashSetAsync();

                List<UserData> validUserDatas = [];

                foreach (var user in userDatas) {
                    if (existingEmails.Contains(user.Email)) {
                        failedImports.Add(new UserDataFailed {
                            Firstname = user.Firstname,
                            Lastname = user.Lastname,
                            Email = user.Email,
                            Role = user.Role,
                            Errors = "Email Already Exists Or Duplicated in Import"
                        });
                    } else {
                        validUserDatas.Add(user);
                        existingEmails.Add(user.Email);
                    }
                }

                await SqlBulkCopyAsync(validUserDatas);

                string fileId = "";
                if (failedImports.Count > 0) {
                    fileId = Guid.NewGuid().ToString();
                    await ExportFailedData(failedImports, fileId);
                }

                await CompleteAsync(jobId, userDatas.Count, validUserDatas.Count, fileId, ct);
            } catch (Exception ex) {
                await FailAsync(jobId, ex.Message);
            }
        }

        public async Task CompleteAsync(string jobId, int total, int passed, string failedFileId = "", CancellationToken ct = default) {
            var job = await GetJobByIdAsync(jobId, ct);
            if (job is null) return;

            job.Total = total;
            job.Passed = passed;
            job.Failed = total - passed;
            job.CompletedAt = DateTime.Now;
            job.Status = passed == total ? BulkImportJobStatus.Completed : BulkImportJobStatus.CompletedWithErrors;
            job.FailedFileId = failedFileId;
            await _context.SaveChangesAsync(ct);
        }

        public async Task FailAsync(string jobId, string errorMessage, CancellationToken ct = default) {
            var job = await GetJobByIdAsync(jobId, ct);
            if (job is null) return;

            job.ErrorMessage = errorMessage;
            job.CompletedAt = DateTime.Now;
            job.Status =BulkImportJobStatus.Failed;
            await _context.SaveChangesAsync(ct);
        }

        public async Task SqlBulkCopyAsync(List<UserData> userDatas, CancellationToken ct = default) {
            await SetDataTables(userDatas);

            string conn_string = _configuration.GetConnectionString("DefaultConnection")!;
            using var connection = new SqlConnection(conn_string);
            await connection.OpenAsync(ct);

            using var usersBulkCopy = new SqlBulkCopy(connection) {
                DestinationTableName = "AspNetUsers",
                BatchSize = batchSize,
                BulkCopyTimeout = 120
            };
            await usersBulkCopy.WriteToServerAsync(_usersDataTable, ct);

            using var rolesBulkCopy = new SqlBulkCopy(connection) {
                DestinationTableName = "AspNetUserRoles",
                BatchSize = batchSize,
                BulkCopyTimeout = 120
            };
            await rolesBulkCopy.WriteToServerAsync(_rolesDataTable, ct);

            _usersDataTable.Rows.Clear();
            _rolesDataTable.Rows.Clear();
        }

        private async Task<List<UserData>> ReadUserDatas(string filePath) {
            using Stream stream = File.OpenRead(filePath);
            if (filePath.EndsWith("csv")) {
                using var reader = new StreamReader(stream);
                using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
                return await csv.GetRecordsAsync<UserData>().ToListAsync();
            } else if (filePath.EndsWith("xlsx")) {
                return (await stream.QueryAsync<UserData>()).ToList();
            }
            return new List<UserData>();
        }

        private async Task ExportFailedData(List<UserDataFailed> usersData, string fileId) {
            foreach (string type in new string[] { "csv", "xlsx" }) {
                string filePath = Path.Combine(_env.WebRootPath, "Storage", "BulkImports", "FailedImports", $"{fileId}.{type}");
                await MiniExcel.SaveAsAsync(filePath, usersData, overwriteFile: true, excelType: type == "csv" ? ExcelType.CSV : ExcelType.XLSX);
            }
        }

        private void InitDataTables() {
            _usersDataTable.Columns.Add("Id", typeof(string));
            _usersDataTable.Columns.Add("FirstName", typeof(string));
            _usersDataTable.Columns.Add("LastName", typeof(string));
            _usersDataTable.Columns.Add("IsActive", typeof(bool));
            _usersDataTable.Columns.Add("Role", typeof(string));
            _usersDataTable.Columns.Add("CreatedAt", typeof(DateTime));
            _usersDataTable.Columns.Add("UserName", typeof(string));
            _usersDataTable.Columns.Add("NormalizedUserName", typeof(string));
            _usersDataTable.Columns.Add("Email", typeof(string));
            _usersDataTable.Columns.Add("NormalizedEmail", typeof(string));
            _usersDataTable.Columns.Add("EmailConfirmed", typeof(bool));
            _usersDataTable.Columns.Add("PasswordHash", typeof(string));
            _usersDataTable.Columns.Add("SecurityStamp", typeof(string));
            _usersDataTable.Columns.Add("ConcurrencyStamp", typeof(string));
            _usersDataTable.Columns.Add("PhoneNumber", typeof(string));
            _usersDataTable.Columns.Add("PhoneNumberConfirmed", typeof(bool));
            _usersDataTable.Columns.Add("TwoFactorEnabled", typeof(bool));
            _usersDataTable.Columns.Add("LockoutEnd", typeof(DateTimeOffset));
            _usersDataTable.Columns.Add("LockoutEnabled", typeof(bool));
            _usersDataTable.Columns.Add("AccessFailedCount", typeof(int));

            _rolesDataTable.Columns.Add("UserId", typeof(string));
            _rolesDataTable.Columns.Add("RoleId", typeof(string));
        }

        private async Task SetDataTables(List<UserData> userDatas) {
            _usersDataTable.Rows.Clear();
            _rolesDataTable.Rows.Clear();

            Dictionary<string, string> roleMap = await _context.Roles.ToDictionaryAsync(r => r.Name!, r => r.Id);

            foreach (var user in userDatas) {
                string userId = Guid.NewGuid().ToString();
                string normalizedEmail = user.Email.ToUpperInvariant();

                _usersDataTable.Rows.Add(
                    userId, // Id
                    user.Firstname, // FirstName
                    user.Lastname, // LastName
                    true, // IsActive
                    user.Role, // Role
                    DateTime.Now, // CreatedAt
                    user.Email, // UserName
                    normalizedEmail, // NormalizedUserName
                    user.Email, // Email
                    normalizedEmail, // NormalizedEmail
                    false, // EmailConfirmed
                    _passwordHasher.HashPassword(null!, user.Password), // Used null coz password hasher doesn't use the object
                    Guid.NewGuid().ToString(), // SecurityStamp
                    Guid.NewGuid().ToString(), // ConcurrencyStamp
                    DBNull.Value, // PhoneNumber
                    false, // PhoneNumberConfirmed
                    false, // TwoFactorEnabled
                    DBNull.Value, // LockoutEnd
                    false, // LockoutEnabled
                    0 // AccessFailedCount
                );
                _rolesDataTable.Rows.Add(
                    userId, // UserId
                    roleMap[user.Role] // RoleId
                );
            }
        }

        private static DateTime getRandomCreatedAt() {
            var start = new DateTime(DateTime.Now.Year, 1, 1);
            var end = new DateTime(DateTime.Now.Year, 12, 31);
            var range = end - start;
            var randomTicks = (long)(Random.Shared.NextDouble() * range.Ticks);
            return start + TimeSpan.FromTicks(randomTicks);
        }

    }
}
