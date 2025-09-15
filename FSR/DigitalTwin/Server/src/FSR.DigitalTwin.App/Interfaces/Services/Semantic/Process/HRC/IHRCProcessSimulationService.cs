using FSR.DigitalTwin.Domain.Model.Process.HRC;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;

public interface IHRCProcessSimulationService
{
    HRCProcessSimulationContext AddModel(Uri client, HRCModel model, string displayName, byte[]? data = null);
    void AddLog(Uri client, HRCProcessSimulationLog log);
}