namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public class InteractionModality
{
    public required Resource Resource { init; get; }
    public required Resource Type { init; get; }
    public required Resource Function1 { init; get; }
    public HRCTask.EAgent Agent1 { init; get; } = HRCTask.EAgent.Any;
    public Resource? Function2 { init; get; }
    public HRCTask.EAgent Agent2 { init; get; } = HRCTask.EAgent.Any;
}