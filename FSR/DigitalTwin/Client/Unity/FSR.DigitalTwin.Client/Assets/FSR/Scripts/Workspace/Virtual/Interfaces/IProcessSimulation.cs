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

        IObservable<ProcessResult> ProcessStarted { init; get; }
        IObservable<ProcessResult> ProcessFinished { init; get; }
        IObservable<Process.Process> ProcessFailed { init; get; }

        bool Initialize(out IProcessSimulationContext context);
        void Run();
        void Reset();

        void Process(IObservable<ProcessResult> process);
    }

    public interface IProcessSimulationContext
    {
        IList<DigitalTwinActorBase> Actors { init; get; }
        IList<SocialOperatorBase> Operators { init; get; }
        IDictionary<Goal, IList<Method>> Goals { init; get; }
        IDictionary<Method, IDictionary<Task, IList<ISet<Task>>>> Methods { init; get; }
        IList<Function> Functions { init; get; }
        IProcessSimulation Simulation { init; get; }

        /* TODO Later add parameters as well... */
    }

}