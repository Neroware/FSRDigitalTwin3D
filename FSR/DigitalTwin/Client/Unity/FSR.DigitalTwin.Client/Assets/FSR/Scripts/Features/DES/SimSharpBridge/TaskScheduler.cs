using System;
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

            return disposable;
        }

        private IDisposable ScheduleGoal<ProcessT>(HRCGoal goal, IObservable<ProcessT> previous, ProcessSimulationBase sim, IProcessSimulationContext ctxt) where ProcessT : HRCProcess
        {
            return null;
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