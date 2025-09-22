using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces
{
    public interface IProcessSimulation
    {
        IObservable<IProcessSimulation> SimulationStarted { get; }
        IObservable<IProcessSimulation> SimulationFinished { get; }
        IObservable<IProcessSimulation> SimulationReset { get; }

        IObservable<Process.Process> ProcessStarted { get; }
        IObservable<ProcessResult> ProcessFinished { get; }
        IObservable<Process.Process> ProcessFailed { get; }

        bool Initialize(out IProcessSimulationContext context);
        void Run();
        void Reset();

        void Process(Process.Process process, IObservable<ProcessResult> processResult);
    }

    public interface IProcessSimulationContext
    {
        IList<DigitalTwinActorBase> Actors { init; get; }
        IList<SocialOperatorBase> Operators { init; get; }
        IDictionary<Goal, IList<Method>> Goals { init; get; }
        IDictionary<Method, IDictionary<Task, IList<ISet<Task>>>> Methods { init; get; }
        IList<Function> Functions { init; get; }
        IProcessSimulation Simulation { set; get; }

        /* TODO Later add parameters as well... */
    }

}