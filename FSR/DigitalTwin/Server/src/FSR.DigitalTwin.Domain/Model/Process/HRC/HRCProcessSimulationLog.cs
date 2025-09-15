namespace FSR.DigitalTwin.Domain.Model.Process.HRC;

public class HRCProcessSimulationLog
{
    public required Uri Id { init; get; }
    public required Uri ClientId { init; get; }
    public required bool Succeeded { init; get; }
    public required long SimulationStart { init; get; }
    public required long SimulationEnd { init; get; }
    public required HRCModel Model { init; get; }
    public byte[] Data { set; get; } = [];
    public override int GetHashCode() => Id.GetHashCode();
}