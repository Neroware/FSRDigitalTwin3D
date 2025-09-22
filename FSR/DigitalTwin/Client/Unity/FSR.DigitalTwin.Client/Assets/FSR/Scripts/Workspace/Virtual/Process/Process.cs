using System;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process
{
    public enum EProcessType
    {
        Event = 0, Goal = 1, Method = 2, Task = 3, Function = 4
    }

    public enum ETaskType
    {
        Basic = 0, Complex = 1
    }

    public record Process
    {
        public virtual EProcessType ProcessType => EProcessType.Event;
        public DateTime Timestamp { set; get; }
        public object[] Inputs { init; get; }
        public object[] InOuts { init; get; }
    }

    public record Goal : Process
    {
        public string GoalId { init; get; }
        public override EProcessType ProcessType => EProcessType.Goal;
        public string GoalName { init; get; }
        public override int GetHashCode() => GoalId.GetHashCode();
    }

    public record Method : Process
    {
        public int MethodId { init; get; }
        public Goal Goal { init; get; }
        public override EProcessType ProcessType => EProcessType.Method;
    }

    public record Task : Process
    {
        private ETaskType _type;
        public Task(ETaskType type)
        {
            _type = type;
        }
        public string TaskId { init; get; }
        public override EProcessType ProcessType => EProcessType.Task;
        public ETaskType TaskType => _type;
        public string Name { set; get; }
        public override int GetHashCode() => TaskId.GetHashCode();
    }

    public record Function : Task
    {
        public Function() : base(ETaskType.Basic) { }
        public override EProcessType ProcessType => EProcessType.Function;
        public IDigitalTwinEntity Actor { init; get; }
        public ISocialOperator Operator { init; get; }
    }
}