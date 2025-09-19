using System;
using System.Collections.Generic;
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
        IList<DigitalTwinActorBase> Actors { init; get; }
        IList<SocialOperatorBase> Operators { init; get; }
        IDictionary<Goal, IList<Method>> Goals { init; get; }
        IDictionary<Method, IList<Process.Process>> Methods { init; get; }
        IDictionary<Task, IList<Process.Process>> Tasks { init; get; }
        IDictionary<Function, SocialOperatorBase> Functions { init; get; }

        /* TODO Later add parameters as well... */
    }

}