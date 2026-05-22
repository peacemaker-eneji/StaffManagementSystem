namespace StaffManagementSystem.Domain {
    public class ApiResponse<T> {
        public int? StatusCode { get; set; }
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "";
        public List<string> Errors { get; set; } = [];
        public T? Data { get; set; }

        public static ApiResponse<T> Fail(params List<string> errors) => new ApiResponse<T> {
            Success = false,
            Errors = errors
        };
    }

    public class ApiResponse : ApiResponse<object>;
}
