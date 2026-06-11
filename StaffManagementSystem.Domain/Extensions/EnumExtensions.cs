using System.ComponentModel;
using System.Reflection;

namespace StaffManagementSystem.Domain.Extensions {
    public static class EnumExtensions {
        public static string? GetDescription(this Enum? theEnum) {
            if (theEnum is null) return null;

            FieldInfo? field = theEnum.GetType().GetField(theEnum.ToString());
            var attribute = field?.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? theEnum.ToString();
        }
    }
}
