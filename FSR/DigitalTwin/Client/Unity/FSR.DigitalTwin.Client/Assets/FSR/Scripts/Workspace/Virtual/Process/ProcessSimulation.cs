using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using UniRx;
using UnityEngine;

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
            public IProcessSimulation Simulation { get; set; }
        }

        public IObservable<IProcessSimulation> SimulationStarted => _simulationStarted;
        public IObservable<IProcessSimulation> SimulationFinished => _simulationFinished;
        public IObservable<IProcessSimulation> SimulationReset => _simulationReset;
        public IObservable<Process> ProcessStarted => _processStarted;
        public IObservable<ProcessResult> ProcessFinished => _processFinished;
        public IObservable<Process> ProcessFailed => _processFailed;

        private Subject<IProcessSimulation> _simulationStarted = new();
        private Subject<IProcessSimulation> _simulationFinished = new();
        private Subject<IProcessSimulation> _simulationReset = new();
        private Subject<Process> _processStarted = new();
        private Subject<ProcessResult> _processFinished = new();
        private Subject<Process> _processFailed = new();

        public bool Initialize(out IProcessSimulationContext context)
        {
            try
            {
                context = DigitalWorkspace.Instance.Knowledge.GetContext();
                context.Simulation = this;
                DoInitialize(context);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                context = null;
                return false;
            }
        }

        public bool LaunchFunction(string functionId, IProcessSimulationContext context, out Function function, IObservable<FunctionResult> success = null, IObservable<Function> failure = null)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionFailed(Function function)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionSucceeded(FunctionResult result)
        {
            throw new NotImplementedException();
        }

        public void Process(Process process, IObservable<ProcessResult> processResult)
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

        private void DoInitialize(IProcessSimulationContext context)
        {
            // Transfer process decomposition to observable data streams
            
        }
    }

}