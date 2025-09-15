using FSR.DigitalTwin.Domain.Model.Process.HRC;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;

public interface IHRCSimulationState {
    ISet<HRCProcessSimulationContext> Contexts { init; get; }
    IDictionary<Uri, List<HRCProcessSimulationLog>> Logs { init; get; }
}

