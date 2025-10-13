using System;
using System.Linq;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    /// <summary>
    /// A class for a naive scheduling strategy. Scheduling is a non-trivial problem, 
    /// however, I lack the time to create a proper scheduler. Therefore, I'll use a naive scheduling strategy
    /// that just always selects the first offered method and task disjunction.
    /// <br/><br/>
    /// NOTE: At some point a scheduler could query the ROS2 scheduler from the sharework project...
    /// </summary>
    public class NaiveTaskScheduler : ITaskScheduler
    {
        public IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            CompositeDisposable disposable = new();
            var prev = sim.SimulationStarted.AsSingleUnitObservable();
            var goalFinished = sim.ProcessFinished
                .Where(p => p.Process.ProcessType == EHRCProcessType.Goal)
                .Select(p => p.Process as HRCGoal);
            foreach(HRCGoal goal in ctxt.Goals.Keys)
            {
                disposable.Add(ScheduleGoal(goal, prev, sim, ctxt));
                prev = goalFinished.Where(g => g.GoalId == goal.GoalId).AsSingleUnitObservable();
            }
            return disposable;
        }
        private IDisposable ScheduleGoal(HRCGoal goal, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            // The naive scheduler always selects the first method given!
            var myMethod = ctxt.Goals[goal].First();
            return ScheduleMethod(myMethod, previous, sim, ctxt);
        }
        private IDisposable ScheduleMethod(HRCMethod method, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            CompositeDisposable disposable = new();
            var methodTasks = ctxt.Methods[method].Keys
                .Where(task => !ctxt.Methods[method].Values
                    .Any(x => x.Any(x => x.Any(x => x.TaskId == task.TaskId))));
            var prev = previous;
            foreach(HRCTask task in methodTasks)
            {
                disposable.Add(ScheduleTask(task, method, prev, sim, ctxt));
                prev = sim.ProcessFinished.Where(p => (p.Process as HRCTask)?.TaskId == task.TaskId).AsSingleUnitObservable();
            }
            return disposable;
        }
        private IDisposable ScheduleTask(HRCTask task, HRCMethod method, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            if (task.ProcessType == EHRCProcessType.Function)
            {
                return ScheduleFunction(task as HRCFunction, previous, sim);
            }
            else if (task.TaskDescription == null || task.TaskDescription.TaskType == EHRCTaskType.Basic)
            {
                CompositeDisposable disposable = new();
                var prev = previous;
                // The naive scheduler always selects the first alternative given!
                foreach (var subTask in ctxt.Methods[method][task].First())
                {
                    disposable.Add(ScheduleTask(subTask, method, prev, sim, ctxt));
                    prev = sim.ProcessFinished.Where(p => (p.Process as HRCTask)?.TaskId == subTask.TaskId).AsSingleUnitObservable();
                }
                return disposable;
            }
            else if (task.TaskDescription.TaskType == EHRCTaskType.Sequential && task.TaskDescription.Constraints.Any(x => x is HRCPrecidenceConstraint))
            {
                CompositeDisposable disposable = new();
                var constraint = task.TaskDescription.Constraints.First(x => x is HRCPrecidenceConstraint) as HRCPrecidenceConstraint;
                var function1 = ctxt.Methods[method][task].First().Where(t => t.TaskId == constraint.First).First();
                var function2 = ctxt.Methods[method][task].First().Where(t => t.TaskId == constraint.Second).First();
                disposable.Add(ScheduleTask(function1, method, previous, sim, ctxt));
                disposable.Add(ScheduleTask(function2, method, sim.ProcessFinished.Where(p => (p.Process as HRCTask)?.TaskId == function1.TaskId)
                    .AsSingleUnitObservable(), sim, ctxt));
                return disposable;
            }
            else if (task.TaskDescription.TaskType == EHRCTaskType.Sequential)
            {
                CompositeDisposable disposable = new();
                var function1 = ctxt.Methods[method][task].First().First();
                var function2 = ctxt.Methods[method][task].First().Skip(1).First();
                disposable.Add(ScheduleTask(function1, method, previous, sim, ctxt));
                disposable.Add(ScheduleTask(function2, method, sim.ProcessFinished.Where(p => (p.Process as HRCTask)?.TaskId == function1.TaskId)
                    .AsSingleUnitObservable(), sim, ctxt));
                return disposable;
            }
            else
            {
                CompositeDisposable disposable = new();
                foreach (var subTask in ctxt.Methods[method][task].First())
                {
                    disposable.Add(ScheduleTask(subTask, method, previous, sim, ctxt));
                }
                return disposable;
            }
            
        }
        private IDisposable ScheduleFunction(HRCFunction function, IObservable<Unit> previous, ProcessSimulationBase sim)
        {
            return previous.Subscribe(_ => sim.Process(function));
        }
    }
}