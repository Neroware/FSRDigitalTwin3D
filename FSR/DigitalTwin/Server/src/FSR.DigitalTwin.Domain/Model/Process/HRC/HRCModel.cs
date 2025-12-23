using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

namespace FSR.DigitalTwin.Domain.Model.Process.HRC;

public class HRCModel {
    private readonly float _horizon;
    private readonly Dictionary<Resource, HRCTask> _tasks = [];
    private readonly Dictionary<Resource, HRCTask> _robotTasks = [];
    private readonly Dictionary<Resource, HRCTask> _humanTasks = [];
    private readonly Dictionary<Resource, AgentSkill> _skills = [];

    public IList<HRCTask> Tasks => [.. _tasks.Values];
    public IList<HRCTask> RobotTasks => [.. _robotTasks.Values];
    public IList<HRCTask> HumanTasks => [.. _humanTasks.Values];
    public List<Resource> Goals { init; get; } = [];
    public IList<AgentSkill> Skills => [.. _skills.Values];

    public HRCModel(float horizon)
    {
        _horizon = horizon;
    }

    public HRCTask CreateRobotTask(Resource function, Resource type) {
        HRCTask task = new(function, type, _horizon);
        _tasks.Add(task.Resource, task);
        _robotTasks.Add(task.Resource, task);
        task.Agent = HRCTask.EAgent.Robot;
        return task;
    }

    public HRCTask CreateHumanTask(Resource function, Resource type) {
        HRCTask task = new(function, type, _horizon);
        _tasks.Add(task.Resource, task);
        _humanTasks.Add(task.Resource, task);
        task.Agent = HRCTask.EAgent.Human;
        return task;
    }

    public HRCTask CreateHRCTask(Resource function, Resource type) {
        HRCTask task = new(function, type, _horizon);
        _tasks.Add(task.Resource, task);
        return task;
    }

    public AgentSkill CreateAgentSkill(Resource skill_, Resource capability, List<Resource> methods)
    {
        AgentSkill skill = new(skill_)
        {
            Capability = capability,
            Methods = methods
        };
        _skills.Add(skill.Resource, skill);
        return skill;
    }
}