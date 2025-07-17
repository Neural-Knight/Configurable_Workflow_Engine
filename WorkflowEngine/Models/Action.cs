using System.Collections.Generic;

namespace WorkflowEngine.Models
{
    public class Action
    {
        public string Id
        {
            get;
            set;
        } // id / name
        public bool Enabled
        {
            get;
            set;
        } // enabled (bool)
        public List<string> FromStates
        {
            get;
            set;
        } = new List<string>(); // fromStates (collection of state IDs)
        public string ToState
        {
            get;
            set;
        } // toState (single state ID)
    }
}