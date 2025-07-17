using System.Collections.Generic;

namespace WorkflowEngine.Models {
    public class WorkflowDefinition
    {
        public string Id
        {
            get;
            set;
        } // A unique ID for the definition
        public string Name
        {
            set;
            get;
        } // Collection of States
        public List<State> States
        {
            get;
            set;
        } = new List<State>(); 
        public List<Action> Actions // and Actions
        {
            get;
            set;
        } = new List<Action>();
    }
}