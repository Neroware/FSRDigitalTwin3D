namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public class InteractionModality
{
    public required Resource Resource { init; get; }
    public required Resource Type { init; get; }
    public Resource? FirstFunction { init; get; }
    public Resource? SecondFunction { init; get; }
    public Resource? HumanFunction { init; get; }
    public Resource? RobotFunction { init; get; }
}