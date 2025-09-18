using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using SimulationServer = FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService.HRCProcessSimulationService.HRCProcessSimulationServiceClient;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process
{
    public class ProcessSimulation : IProcessSimulation
    {
        public class ProcessSimulationContext : IProcessSimulationContext
        {
            private readonly SimulationServer _server;
            public ProcessSimulationContext(SimulationServer server)
            {
                _server = server;
            }
            public SimulationServer SimulationService => _server;
            public IList<IDigitalTwinEntity> Actors { get; init; } = new List<IDigitalTwinEntity>();
            public IList<ISocialOperator> Operators { get; init; } = new List<ISocialOperator>();
            public IDictionary<Goal, IList<Method>> Goals { get; init; } = new Dictionary<Goal, IList<Method>>();
            public IDictionary<Method, IList<ISet<Process>>> Methods { get; init; } = new Dictionary<Method, IList<ISet<Process>>>();
            public IDictionary<ISocialOperator, Function> Functions { get; init; } = new Dictionary<ISocialOperator, Function>();

        }

        public IObservable<IProcessSimulation> SimulationStarted => throw new NotImplementedException();
        public IObservable<IProcessSimulation> SimulationFinished => throw new NotImplementedException();
        public IObservable<IProcessSimulation> SimulationReset => throw new NotImplementedException();
        public IObservable<ProcessResult> ProcessStarted { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IObservable<ProcessResult> ProcessFinished { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }
        public IObservable<Process> ProcessFailed { get => throw new NotImplementedException(); init => throw new NotImplementedException(); }

        public bool Initialize(out IProcessSimulationContext context)
        {
            throw new NotImplementedException();
        }

        public void Process(IObservable<ProcessResult> process)
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            throw new NotImplementedException();
        }
    }

}