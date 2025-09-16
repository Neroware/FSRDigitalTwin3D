namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public class HRCTask
{
    public enum EAgent
    {
        Any = 0, Human = 1, Robot = 2
    }

    private readonly long _horizon;
    public Resource Resource { get; }
    public Resource Type { get; }
    public Resource? Target { set; get; }

    public string Name { set; get; }
    public string? Description { set; get; }
    public string? Goal { set; get; }
    public string? Start { set; get; }
    public EAgent Agent { set; get; } = EAgent.Any;
    public Tuple<long, long> Duration => new(
        Math.Max(1, AverageDuration - DurationUncertainty),
        Math.Min(AverageDuration + DurationUncertainty, _horizon)
    );
    public long AverageDuration { set; get; } = 1;
    public long DurationUncertainty { set; get; }
    public double SuccessRate { set; get; } = 0.99;

    public HRCTask(Resource function, Resource type, long horizon)
    {
        Name = function.ToString();
        Description = Name;
        Resource = function;
        Type = type;
        DurationUncertainty = horizon;
        _horizon = horizon;
    }

    public override string ToString()
    {
        return "RobotFunction{" +
                "function=" + Resource +
                '}';
    }
}