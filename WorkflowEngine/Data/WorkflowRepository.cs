using System.Collections.Generic;
using WorkflowEngine.Models;
using System.Linq;

namespace WorkflowEngine.Data
{
    public static class WorkflowRepository
    {
        // Stores workflow definitions by their ID
        public static Dictionary<string, WorkflowDefinition> Definitions
        {
            get;
        } = new Dictionary<string, WorkflowDefinition>();

        // Stores workflow instances by their ID
        public static Dictionary<string, WorkflowInstance> Instances
        {
            get;
        } = new Dictionary<string, WorkflowInstance>();

        // --- Definition Operations ---

        public static void AddDefinition(WorkflowDefinition definition)
        {
            Definitions.Add(definition.Id, definition);
        }

        public static WorkflowDefinition GetDefinition(string definitionId)
        {
            Definitions.TryGetValue(definitionId, out var definition);
            return definition;
        }

        // --- Instance Operations ---

        public static void AddInstance(WorkflowInstance instance)
        {
            Instances.Add(instance.Id, instance);
        }

        public static WorkflowInstance GetInstance(string instanceId)
        {
            Instances.TryGetValue(instanceId, out var instance);
            return instance;
        }

        public static IEnumerable<WorkflowInstance> GetAllInstances()
        {
            return Instances.Values;
        }
    }
}