using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

namespace FSR.DigitalTwin.Domain.Model.Process.HRC;

public class HRCModel {
    private readonly long _horizon;
    private readonly Dictionary<Resource, HRCTask> _tasks = [];
    private readonly Dictionary<Resource, HRCTask> _robotTasks = [];
    private readonly Dictionary<Resource, HRCTask> _humanTasks = [];

    public IList<HRCTask> Tasks => [.. _tasks.Values];
    public IList<HRCTask> RobotTasks => [.. _robotTasks.Values];
    public IList<HRCTask> HumanTasks => [.. _humanTasks.Values];

    public List<Resource> Goals { init; get; } = [];

    public HRCModel(long horizon)
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
}