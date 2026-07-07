using Microsoft.Extensions.AI;

namespace StaffManagementSystem.Domain.Interfaces {
    public interface IAiToolsStore {

        // Scans a tool class instance, extracts its public methods, and indexes them.
        void RegisterTools<T>(T toolInstance, string storeCategory) where T : class;

        // Fetches all registered tool configurations for specified categories.
        List<AIFunction> GetTools(params string[] categories);
    }
}
