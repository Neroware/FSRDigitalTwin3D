using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services.Semantic.Process;

public class SkillBasedProgrammingService : ISkillBasedProgrammingService
{
    private readonly IOntologyModelService _ontology;
    private readonly IHRCKnowledgeService _knowledgeBase;
    private readonly ILogger<SkillBasedProgrammingService> _logger;
    public SkillBasedProgrammingService(IOntologyModelService ontology, IHRCKnowledgeService knowledgeBase, ILogger<SkillBasedProgrammingService> logger)
    {
        _ontology = ontology ?? throw new ArgumentNullException(nameof(ontology));
        _knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }
    public IEnumerable<Resource> GetAgents() => _knowledgeBase.GetAgents();
    public IEnumerable<Resource> GetCapabilities(Resource agent) => throw new NotImplementedException();
    public IEnumerable<Resource> GetDevicePrimitives(Resource agentSkill) => throw new NotImplementedException();
    public IEnumerable<AgentSkill> GetSkills()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetSkillsQuery { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }
}