using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.HRC;

public class HRCSimulationState : IHRCSimulationState
{
    public ISet<HRCProcessSimulationContext> Contexts { get; init; } = new HashSet<HRCProcessSimulationContext>();
    public IDictionary<Uri, List<HRCProcessSimulationLog>> Logs { get; init; } = new Dictionary<Uri, List<HRCProcessSimulationLog>>();
}