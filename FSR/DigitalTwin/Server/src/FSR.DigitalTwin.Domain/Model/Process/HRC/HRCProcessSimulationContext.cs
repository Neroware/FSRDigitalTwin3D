namespace FSR.DigitalTwin.Domain.Model.Process.HRC;

public class HRCProcessSimulationContext
{
    public required Uri Id { init; get; }
    public required Uri ClientId { init; get; }
    public required string DisplayName { init; get; }
    public required HRCModel Model { init; get; }
    public byte[] Data { set; get; } = [];
    public override int GetHashCode() => Id.GetHashCode();
}