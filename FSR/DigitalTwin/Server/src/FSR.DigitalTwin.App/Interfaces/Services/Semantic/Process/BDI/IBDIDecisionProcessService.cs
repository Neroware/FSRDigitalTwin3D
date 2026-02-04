using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.BDI;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.BDI;

public interface IBDIDecisionProcessService
{
    public BDIActionIntent GetDecisionProcessActionIntent(Resource decisionProcess);
}