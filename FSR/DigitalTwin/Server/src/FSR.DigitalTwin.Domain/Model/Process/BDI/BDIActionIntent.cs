namespace FSR.DigitalTwin.Domain.Model.Process.BDI;

public class BDIActionIntent
{
    public required Resource Resource { get; init; }
    public Resource? Decision { get; init; }
    public List<Resource> Tasks { get; init; } = [];
}