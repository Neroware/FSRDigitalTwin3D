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
        Basic = 0, Complex = 1, Conjuctive = 2, Disjunctive = 3
    }

    public record Process
    {
        public Uri ProcessId { init; get; }
        public virtual EProcessType ProcessType => EProcessType.Event;
        public DateTime Timestamp { init; get; }
        public object[] Inputs { init; get; }
        public object[] InOuts { init; get; }
        public override int GetHashCode() => ProcessId.ToString().GetHashCode();
    }

    public record Goal : Process
    {
        public override EProcessType ProcessType => EProcessType.Goal;
        public string GoalName { init; get; }
    }

    public record Method : Process
    {
        public override EProcessType ProcessType => EProcessType.Method;
        public Goal Goal { init; get; }
        public string MethodName { init; get; }
    }

    public record Task : Process
    {
        public override EProcessType ProcessType => EProcessType.Task;
        public ETaskType TaskType { init; get; }
        public string TaskName { init; get; }
    }

    public record Function : Process
    {
        public override EProcessType ProcessType => EProcessType.Function;
        public string FunctionName { init; get; }
        public IDigitalTwinEntity Actor { init; get; }
        public ISocialOperator Operator { init; get; }
    }
}