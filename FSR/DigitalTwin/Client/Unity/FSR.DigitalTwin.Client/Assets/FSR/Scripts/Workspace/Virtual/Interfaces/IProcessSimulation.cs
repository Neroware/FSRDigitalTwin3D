using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process;
using static FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService.HRCProcessSimulationService;


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
        HRCProcessSimulationServiceClient SimulationService { get; }
        IList<IDigitalTwinEntity> Actors { init; get; }
        IList<ISocialOperator> Operators { init; get; }
        IDictionary<Goal, IList<Method>> Goals { init; get; }
        IDictionary<Method, IList<ISet<Process.Process>>> Methods { init; get; }
        IDictionary<ISocialOperator, Function> Functions { init; get; }

        /* TODO Later add parameters as well... */
    }

}