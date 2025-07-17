using System;
using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class WorkflowInstance
    {
        public string Id
        {
            get;
            set;
        } // Unique ID for this instance
        public string WorkflowDefinitionId
        {
            get;
            set;
        } // Reference to its definition
        public string CurrentStateId
        {
            get;
            set;
        } // current state
        public List<WorkflowHistoryEntry> History
        {
            get;
            set;
        } = new List<WorkflowHistoryEntry>(); // basic history (action + timestamp)
    }

    public class WorkflowHistoryEntry
    {
        public string ActionId
        {
            get;
            set;
        }
        public DateTime Timestamp
        {
            get;
            set;
        }
        public string FromStateId
        {
            get;
            set;
        }
        public string ToStateId
        {
            get;
            set;
        }
    }
}