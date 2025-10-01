namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public class HRCTask
{
    public enum EAgent
    {
        Any = 0, Human = 1, Robot = 2
    }

    private readonly float _horizon;
    public Resource Resource { get; }
    public Resource Type { get; }
    public Resource? Target { set; get; }

    public string Name { set; get; }
    public long Id { set; get; }
    public string? Description { set; get; }
    public string? Goal { set; get; }
    public string? StartLocation { set; get; }
    public string? EndLocation { set; get; }
    public string? Location { set; get; }
    public EAgent Agent { set; get; } = EAgent.Any;
    public Tuple<float, float> Duration => new(
        Math.Max(1, AverageDuration - DurationUncertainty),
        Math.Min(AverageDuration + DurationUncertainty, _horizon)
    );
    public float AverageDuration { set; get; } = 1;
    public float DurationUncertainty { set; get; }
    public double SuccessRate { set; get; } = 0.99;

    public HRCTask(Resource function, Resource type, float horizon)
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