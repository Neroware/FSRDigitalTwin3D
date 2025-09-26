using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public enum EHRCProcessType
    {
        Event = 0, Goal = 1, Method = 2, Task = 3, Function = 4
    }

    public enum EHRCTaskType
    {
        Basic = 0,
        Independent = 1,
        Simultaneous = 2,
        Supportive = 3,
        Synchronous = 4,
        Complex = 5,
        Conjuctive = 6,
        Disjunctive = 7,
    }

    public record HRCProcess
    {
        public virtual EHRCProcessType ProcessType => EHRCProcessType.Event;
        public DateTime Timestamp { set; get; }
        public object[] Inputs { init; get; }
        public object[] InOuts { init; get; }
    }

    public record HRCGoal : HRCProcess
    {
        public string GoalId { init; get; }
        public override EHRCProcessType ProcessType => EHRCProcessType.Goal;
        public string GoalName { init; get; }
        public override int GetHashCode() => GoalId.GetHashCode();
    }

    public record HRCMethod : HRCProcess
    {
        public int MethodId { init; get; }
        public HRCGoal Goal { init; get; }
        public override EHRCProcessType ProcessType => EHRCProcessType.Method;
    }

    public record HRCTask : HRCProcess
    {
        public string TaskId { init; get; }
        public override EHRCProcessType ProcessType => EHRCProcessType.Task;
        public HRCTaskDescription TaskDescription { set; get; } = null;
        public override int GetHashCode() => TaskId.GetHashCode();
    }

    public record HRCTaskDescription
    {
        public EHRCTaskType TaskType { init; get; }
        public string TaskName { init; get; }
    }

    public record HRCFunction : HRCTask
    {
        public override EHRCProcessType ProcessType => EHRCProcessType.Function;
        public IDigitalTwinEntity Actor { init; get; }
        public ISocialOperator Operator { init; get; }
        public HRCFunctionDescription FunctionDescription => TaskDescription as HRCFunctionDescription;
    }

    public record HRCFunctionDescription : HRCTaskDescription
    {
        public TimeSpan Duration { init; get; }
        public TimeSpan DurationUncertainty { init; get; }
        public double SuccessRate { init; get; }
    }
}