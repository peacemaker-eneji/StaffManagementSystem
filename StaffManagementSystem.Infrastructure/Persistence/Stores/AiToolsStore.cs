using Microsoft.Extensions.AI;
using StaffManagementSystem.Domain.Interfaces;
using System.Reflection;

namespace StaffManagementSystem.Infrastructure.Persistence.Stores {
    public class AiToolsStore : IAiToolsStore {
        // Thread-safe dictionary storing discovered functions organized by a custom domain key
        private readonly Dictionary<string, List<AIFunction>> _registry = new();
        private readonly List<AIFunction> _allTools = new();

        // Scans a tool class instance, extracts its public methods, and indexes them.
        public void RegisterTools<T>(T toolInstance, string storeCategory) where T : class {
            var discoveredFunctions = new List<AIFunction>();

            // Extract all custom methods belonging to this specific instance
            var methods = typeof(T).GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);

            foreach (var method in methods) {
                var aiFunction = AIFunctionFactory.Create(method, toolInstance);
                discoveredFunctions.Add(aiFunction);
            }

            storeCategory = storeCategory.ToLowerInvariant();
            if (!_registry.ContainsKey(storeCategory)) _registry[storeCategory] = new List<AIFunction>();
            _registry[storeCategory].AddRange(discoveredFunctions);
            _allTools.AddRange(discoveredFunctions);
        }

        // Fetches all registered tool configurations for specified categories.
        public List<AIFunction> GetTools(params string[] categories) {
            if (categories.Count() == 0) return _allTools;

            var toolPackage = new List<AIFunction>();
            foreach (var category in categories) {
                if (_registry.TryGetValue(category.ToLowerInvariant(), out var functions)) {
                    toolPackage.AddRange(functions);
                }
            }

            return toolPackage;
        }
    }
}
