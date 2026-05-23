namespace StaffManagementSystem.Domain.Models {
    public class ApiResponse<T> {
        public int Status { get; set; } = 200;
        public bool Success { get; set; } = true;
        public string Message { get; set; } = "";
        public List<string> Errors { get; set; } = [];
        public T? Data { get; set; }
    }

    public class ApiResponse : ApiResponse<object>;
}
