using StaffManagementSystem.Domain.Enums;

namespace StaffManagementSystem.Domain.Models {
    public record UserInfo(
        string Id,
        string Firstname,
        string Lastname,
        string Email,
        bool IsActive,
        Role Role,
        DateTime CreatedAt
    );
}
