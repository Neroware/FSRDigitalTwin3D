using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using Microsoft.Extensions.Logging;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.HRC;

public class HRCKnowledgeAuthoringService
{
    private readonly IHRCKnowledgeService _knowledgeBase;
    private readonly ILogger<HRCKnowledgeAuthoringService> _logger;

    public HRCKnowledgeAuthoringService(IHRCKnowledgeService knowledgeBase, ILogger<HRCKnowledgeAuthoringService> logger)
    {
        _knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public HRCModel CreateModel(long horizon)
    {
        HRCModel hrc = new(horizon);

        var humans = _knowledgeBase.GetHumans()
            .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
        foreach (var human in humans)
        {
            var functions = _knowledgeBase.GetFunctionsByAgent(human.Uri)
                .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
            foreach (var function in functions)
            {
                hrc.CreateHumanTask(function, _knowledgeBase.GetResourceType(function.Uri));
            }
        }

        var robots = _knowledgeBase.GetCobots()
            .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
        foreach (var robot in robots)
        {
            var functions = _knowledgeBase.GetFunctionsByAgent(robot.Uri)
                .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
            foreach (var function in functions)
            {
                hrc.CreateRobotTask(function, _knowledgeBase.GetResourceType(function.Uri));
            }
        }

        var goals = _knowledgeBase.GetGoals()
            .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
        hrc.Goals.AddRange(goals);

        return hrc;
    }
}