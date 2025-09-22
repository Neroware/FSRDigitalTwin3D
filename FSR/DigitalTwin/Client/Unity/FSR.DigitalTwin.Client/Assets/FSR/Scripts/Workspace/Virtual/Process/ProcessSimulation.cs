using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process
{
    public class ProcessSimulation : IProcessSimulation
    {
        public class ProcessSimulationContext : IProcessSimulationContext
        {
            public IList<DigitalTwinActorBase> Actors { get; init; } = new List<DigitalTwinActorBase>();
            public IList<SocialOperatorBase> Operators { get; init; } = new List<SocialOperatorBase>();
            public IDictionary<Goal, IList<Method>> Goals { get; init; } = new Dictionary<Goal, IList<Method>>();
            public IDictionary<Method, IDictionary<Task, IList<ISet<Task>>>> Methods { get; init; } = new Dictionary<Method, IDictionary<Task, IList<ISet<Task>>>>();
            public IList<Function> Functions { get; init; } = new List<Function>();
            public IProcessSimulation Simulation { get; init; }
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