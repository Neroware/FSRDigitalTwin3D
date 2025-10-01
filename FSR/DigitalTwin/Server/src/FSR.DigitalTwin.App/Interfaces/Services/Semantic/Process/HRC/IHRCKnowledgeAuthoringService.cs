using FSR.DigitalTwin.Domain.Model.Process.HRC;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;

public interface IHRCKnowledgeAuthoringService
{
    public HRCModel CreateModel(float horizon);
    public void AttachMetadata(HRCModel model);
}