// Services/WorkflowService.cs
using System;
using System.Linq;
using System.Collections.Generic;
using WorkflowEngine.Models;
using WorkflowEngine.Data;

namespace WorkflowEngine.Services
{
    public class WorkflowService
    {
        public (WorkflowDefinition definition, string error) CreateWorkflowDefinition(WorkflowDefinition newDefinition)
        {
            // Validation rules
            if (string.IsNullOrWhiteSpace(newDefinition.Id))
            {
                return (null, "Workflow Definition ID cannot be empty.");
            }
            if (WorkflowRepository.Definitions.ContainsKey(newDefinition.Id))
            {
                return (null, $"Workflow Definition with ID '{newDefinition.Id}' already exists.");
            }

            var initialStates = newDefinition.States.Where(s => s.IsInitial).ToList();
            if (initialStates.Count != 1) // Must contain exactly one isInitial == true state.
            {
                return (null, "A workflow definition must contain exactly one initial state.");
            }

            // Check for duplicate state IDs
            if (newDefinition.States.GroupBy(s => s.Id).Any(g => g.Count() > 1))
            {
                return (null, "Workflow Definition contains duplicate state IDs.");
            }

            // Check for duplicate action IDs
            if (newDefinition.Actions.GroupBy(a => a.Id).Any(g => g.Count() > 1))
            {
                return (null, "Workflow Definition contains duplicate action IDs.");
            }

            // Validate all 'toState' references in actions are valid state IDs
            foreach (var action in newDefinition.Actions)
            {
                if (!newDefinition.States.Any(s => s.Id == action.ToState))
                {
                    return (null, $"Action '{action.Id}' references an unknown 'ToState' ID: '{action.ToState}'.");
                }
                // Validate all 'fromStates' references in actions are valid state IDs
                foreach (var fromStateId in action.FromStates)
                {
                    if (!newDefinition.States.Any(s => s.Id == fromStateId))
                    {
                        return (null, $"Action '{action.Id}' references an unknown 'FromState' ID: '{fromStateId}'.");
                    }
                }
            }

            WorkflowRepository.AddDefinition(newDefinition); // Create a new workflow definition
            return (newDefinition, null);
        }

        public WorkflowDefinition GetWorkflowDefinition(string definitionId)
        {
            return WorkflowRepository.GetDefinition(definitionId); // Retrieve an existing definition
        }

        public (WorkflowInstance instance, string error) StartWorkflowInstance(string definitionId)
        {
            var definition = WorkflowRepository.GetDefinition(definitionId);
            if (definition == null)
            {
                return (null, $"Workflow Definition with ID '{definitionId}' not found.");
            }

            var initialState = definition.States.FirstOrDefault(s => s.IsInitial);
            if (initialState == null)
            {
                // This should ideally be caught by definition validation, but as a safeguard.
                return (null, $"Workflow Definition '{definitionId}' has no initial state.");
            }

            var instance = new WorkflowInstance
            {
                Id = Guid.NewGuid().ToString(), // Generate a unique ID for the instance
                WorkflowDefinitionId = definitionId, // Reference to its definition
                CurrentStateId = initialState.Id, // Starts at the definition's initial state.
                History = new List<WorkflowHistoryEntry>
                {
                    new WorkflowHistoryEntry {
                        ActionId = "START",
                        Timestamp = DateTime.UtcNow,
                        FromStateId = null,
                        ToStateId = initialState.Id
                    }
                }
            };

            WorkflowRepository.AddInstance(instance); // Start a new workflow instance for a chosen definition.
            return (instance, null);
        }

        public (WorkflowInstance instance, string error) ExecuteAction(string instanceId, string actionId)
        {
            var instance = WorkflowRepository.GetInstance(instanceId);
            if (instance == null)
            {
                return (null, $"Workflow Instance with ID '{instanceId}' not found.");
            }

            var definition = WorkflowRepository.GetDefinition(instance.WorkflowDefinitionId);
            if (definition == null)
            {
                // This indicates data inconsistency, should not happen if definitions are properly managed
                return (null, $"Workflow Definition '{instance.WorkflowDefinitionId}' for instance '{instanceId}' not found.");
            }

            var action = definition.Actions.FirstOrDefault(a => a.Id == actionId);
            if (action == null)
            {
                return (null, $"Action '{actionId}' does not belong to workflow definition '{definition.Id}'.");
            }

            if (!action.Enabled)
            {
                return (null, $"Action '{actionId}' is disabled.");
            }

            // Check if the current state is in fromStates
            if (!action.FromStates.Contains(instance.CurrentStateId))
            {
                return (null, $"Current state '{instance.CurrentStateId}' is not a valid 'fromState' for action '{actionId}'.");
            }

            // Check if current state is a final state (cannot transition from final states)
            var currentState = definition.States.FirstOrDefault(s => s.Id == instance.CurrentStateId);
            if (currentState != null && currentState.IsFinal)
            {
                return (null, $"Cannot execute action '{actionId}' from a final state '{instance.CurrentStateId}'.");
            }

            // Perform the state transition
            var oldStateId = instance.CurrentStateId;
            instance.CurrentStateId = action.ToState; // moving it to the target state

            // Add to history
            instance.History.Add(new WorkflowHistoryEntry
            {
                ActionId = action.Id,
                Timestamp = DateTime.UtcNow,
                FromStateId = oldStateId,
                ToStateId = instance.CurrentStateId
            });

            return (instance, null);
        }

        public WorkflowInstance GetWorkflowInstance(string instanceId)
        {
            return WorkflowRepository.GetInstance(instanceId); // Retrieve the current state (and basic history) of an instance.
        }

        public IEnumerable<WorkflowInstance> GetAllWorkflowInstances()
        {
            return WorkflowRepository.GetAllInstances(); // Inspect/list running instances.
        }

        public IEnumerable<WorkflowDefinition> GetAllWorkflowDefinitions()
        {
            return WorkflowRepository.Definitions.Values; // Inspect/list definitions.
        }
    }
}