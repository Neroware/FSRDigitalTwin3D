namespace FSR.DigitalTwin.Domain.Model;

public record Individual
{
    public required Resource Resource { init; get; }
    public required HashSet<Resource> Type { init; get; }
}