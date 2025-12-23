using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using Microsoft.Extensions.Logging;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.HRC;

public class HRCKnowledgeAuthoringService : IHRCKnowledgeAuthoringService
{
    private readonly IHRCKnowledgeService _knowledgeBase;
    private readonly ISkillBasedProgrammingService _skillBase;
    private readonly ILogger<HRCKnowledgeAuthoringService> _logger;

    public HRCKnowledgeAuthoringService(IHRCKnowledgeService knowledgeBase, ISkillBasedProgrammingService skillBase, ILogger<HRCKnowledgeAuthoringService> logger)
    {
        _knowledgeBase = knowledgeBase ?? throw new ArgumentNullException(nameof(knowledgeBase));
        _skillBase = skillBase ?? throw new ArgumentNullException(nameof(skillBase));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public HRCModel CreateModel(float horizon)
    {
        HRCModel hrc = new(horizon);

        var humans = _knowledgeBase.GetHumans();
        foreach (var human in humans)
        {
            var functions = _knowledgeBase.GetFunctionsByAgent(human);
            foreach (var function in functions)
            {
                hrc.CreateHumanTask(function, new Domain.Model.Resource() { Uri = UriPrefix.SOHO + "Function"});
            }
        }

        var robots = _knowledgeBase.GetCobots();
        foreach (var robot in robots)
        {
            var functions = _knowledgeBase.GetFunctionsByAgent(robot);
            foreach (var function in functions)
            {
                hrc.CreateRobotTask(function, new Domain.Model.Resource() { Uri = UriPrefix.SOHO + "Function"});
            }
        }

        var goals = _knowledgeBase.GetGoals();
        hrc.Goals.AddRange(goals);

        var skills = _skillBase.GetSkills();
        foreach(var skill in skills)
        {
            hrc.CreateAgentSkill(skill.Resource, 
                skill.Capability ?? throw new NullReferenceException("should not happen"), skill.Methods);
        }

        return hrc;
    }

    public void AttachMetadata(HRCModel model)
    {
        var allTasks = model.HumanTasks.Concat(model.RobotTasks).Concat(model.Tasks).ToHashSet();
        foreach (var task in allTasks)
        {
            var functionObjectData = _knowledgeBase.GetFunctionObjectProperties(task.Resource);
            var functionPropertyData = _knowledgeBase.GetFunctionDataProperties(task.Resource);
            task.AverageDuration = functionPropertyData.Duration;
            task.DurationUncertainty = functionPropertyData.DurationUncertainty;
            task.SuccessRate = 1.0;
            task.Description = functionPropertyData.ProcedureDescription;
            task.Name = functionPropertyData.ProcedureName ?? task.Name;
            task.Id = functionPropertyData.ProcedureId;
            task.Target = functionObjectData.Target.FirstOrDefault();
            task.StartLocation = functionObjectData.StartLocation.Select(res => res.ToString()).FirstOrDefault();
            task.EndLocation = functionObjectData.EndLocation.Select(res => res.ToString()).FirstOrDefault();
            task.Location = functionObjectData.Location.Select(res => res.ToString()).FirstOrDefault();
            task.Goal = functionPropertyData.Goal;
        }
    }
}