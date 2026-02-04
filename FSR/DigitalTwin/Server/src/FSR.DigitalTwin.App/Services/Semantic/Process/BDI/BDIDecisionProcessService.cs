using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.BDI;
using FSR.DigitalTwin.App.Queries.Semantic.Process.BDI;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.BDI;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.BDI;

public class BDIDecisionProcessService : IBDIDecisionProcessService
{
    private IOntologyModelService _ontology;
    private ILogger<BDIDecisionProcessService> _logger;

    public BDIDecisionProcessService(IOntologyModelService ontology, ILogger<BDIDecisionProcessService> logger)
    {
        _ontology = ontology ?? throw new ArgumentNullException(nameof(ontology));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public BDIActionIntent GetDecisionProcessActionIntent(Resource decisionProcess)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetActionIntent(decisionProcess) { SparqlServer = server });
        return result.IsSuccess ? result.Value : 
            new BDIActionIntent() { Resource = new Resource() { LocalName = "<<unknown action intent>>"}};
    }
}