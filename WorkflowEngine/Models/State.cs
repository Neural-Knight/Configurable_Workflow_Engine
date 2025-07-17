namespace WorkflowEngine.Models
{
    public class State
    {
        public string Id
        {
            get;
            set;
        } // id / name
        public bool IsInitial
        {
            get;
            set;
        } // isInitial (bool)
        public bool IsFinal
        {
            get;
            set;
        } // isFinal (bool)
        public bool Enabled
        {
            get;
            set;
        } // enabled (bool)
        public string Description
        {
            get;
            set;
        } // description (string)
    }
}