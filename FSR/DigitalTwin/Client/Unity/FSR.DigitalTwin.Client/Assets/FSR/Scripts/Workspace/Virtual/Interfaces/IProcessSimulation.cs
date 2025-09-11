using System;
using System.Collections.Generic;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process;


namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces
{
    public interface IProcessSimulation
    {
        IProcessSimulationContext Context { get; }

        IObservable<UniRx.Unit> SimulationStarted { get; }
        IObservable<UniRx.Unit> SimulationFinished { get; }
        IObservable<UniRx.Unit> SimulationReset { get; }

        IObservable<ProcessResult> ProcessStarted { init; get; }
        IObservable<ProcessResult> ProcessFinished { init; get; }
        IObservable<Process.Process> ProcessFailed { init; get; }

        void Run();
        void Reset();

        void Process(IObservable<ProcessResult> process);
    }

    public interface IProcessSimulationContext
    {
        HRCProcessSimulationService SimulationService { init; get; }
        IList<IDigitalTwinEntity> Actors { init; get; }
        IList<ISocialOperator> Operators { init; get; }
        IDictionary<Goal, List<Method>> Methods { init; get; }

        /* TODO Later add parameters as well... */
    }

}