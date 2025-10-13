using System;
using System.Linq;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    /// <summary>
    /// A class for a naive scheduling strategy. Scheduling is a non-trivial problem, 
    /// however, I lack the time to create a proper scheduler. Therefore, I'll use a naive scheduling strategy
    /// that just always selects the first offered method and task disjunction. This is also the reason why this
    /// class is static and not an interface following the strategy design pattern.
    /// <br/><br/>
    /// NOTE: I need to change this asap!
    /// </summary>
    // public static class TaskScheduler
    // {
    //     public static IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt)
    //     {
    //         // TODO Schedule HRCFunctions with based on InteractionModality
    //         //
    //         // We already did the horizontal dependency of tasks and subtasks, now, we do the vertical (sequential) 
    //         // one using this scheduler class! We do this by sequentially calling ProcessSimulation.Process(...)
    //         // each time the previous process terminated using the observables from ProcessSimulationBase.

    //         return null;
    //     }

    //     private static IObservable<HRCGoal> ScheduleGoal(HRCGoal goal, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     private static IObservable<HRCMethod> ScheduleMethod(HRCMethod method, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     private static IObservable<HRCTask> ScheduleTask(HRCTask task, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
    //     {
    //         throw new NotImplementedException();
    //     }

    //     private static IObservable<HRCFunction> ScheduleFunction(HRCFunction function, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
    //     {
    //         throw new NotImplementedException();
    //     }
    // }

    public class NaiveTaskScheduler : ITaskScheduler
    {
        public IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            CompositeDisposable disposable = new();
            var prev = sim.SimulationStarted.AsSingleUnitObservable();
            foreach(HRCGoal goal in ctxt.Goals.Keys)
            {
                disposable.Add(ScheduleGoal(goal, prev, sim, ctxt));
                prev = sim.ProcessFinished.Where(p => p.Process == goal).AsSingleUnitObservable();
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
                prev = sim.ProcessFinished.Where(p => p.Process == task).AsSingleUnitObservable();
            }
            return disposable;
        }
        private IDisposable ScheduleTask(HRCTask task, HRCMethod method, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            if (task.ProcessType == EHRCProcessType.Function)
            {
                return ScheduleFunction(task as HRCFunction, previous, sim, ctxt);
            }
            else if (task.TaskDescription == null || task.TaskDescription.TaskType == EHRCTaskType.Basic)
            {
                CompositeDisposable disposable = new();
                var prev = previous;
                // The naive scheduler always selects the first alternative given!
                foreach (var subTask in ctxt.Methods[method][task].First())
                {
                    disposable.Add(ScheduleTask(subTask, method, prev, sim, ctxt));
                    prev = sim.ProcessFinished.Where(p => p.Process == subTask).AsSingleUnitObservable();
                }
                return disposable;
            }
            throw new NotImplementedException();
        }
        private IDisposable ScheduleFunction(HRCFunction function, IObservable<Unit> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            throw new NotImplementedException();
        }


        // private IObservable<HRCGoal> ScheduleGoal(HRCGoal goal, CompositeDisposable disp, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        // {
        //     return Observable.Never<HRCGoal>();
        // }

        // private IObservable<HRCMethod> ScheduleMethod(HRCMethod method, CompositeDisposable disp, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        // {
        //     return Observable.Never<HRCMethod>();
        // }

        // private IObservable<HRCTask> ScheduleTask(HRCTask task, CompositeDisposable disp, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        // {
        //     return Observable.Never<HRCTask>();
        // }

        // private IObservable<HRCFunction> ScheduleFunction(HRCTask function, CompositeDisposable disp, ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        // {

        // }
    }
}