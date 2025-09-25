using System;
using System.Collections.Generic;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public class ProcessSimulation : IProcessSimulation
    {
        public class ProcessSimulationContext : IProcessSimulationContext
        {
            public IList<DigitalTwinActorBase> Actors { get; init; } = new List<DigitalTwinActorBase>();
            public IList<SocialOperatorBase> Operators { get; init; } = new List<SocialOperatorBase>();
            public IDictionary<HRCGoal, IList<HRCMethod>> Goals { get; init; } = new Dictionary<HRCGoal, IList<HRCMethod>>();
            public IDictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>> Methods { get; init; } = new Dictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>>();
            public IList<HRCFunction> Functions { get; init; } = new List<HRCFunction>();
            public IProcessSimulation Simulation { get; set; }
        }

        public IObservable<IProcessSimulation> SimulationStarted => _simulationStarted;
        public IObservable<IProcessSimulation> SimulationFinished => _simulationFinished;
        public IObservable<IProcessSimulation> SimulationReset => _simulationReset;
        public IObservable<HRCProcess> ProcessStarted => _processStarted;
        public IObservable<HRCProcessResult<HRCProcess>> ProcessFinished => _processFinished;
        public IObservable<HRCProcess> ProcessFailed => _processFailed;

        private Subject<IProcessSimulation> _simulationStarted = new();
        private Subject<IProcessSimulation> _simulationFinished = new();
        private Subject<IProcessSimulation> _simulationReset = new();
        private Subject<HRCProcess> _processStarted = new();
        private Subject<HRCProcessResult<HRCProcess>> _processFinished = new();
        private Subject<HRCProcess> _processFailed = new();

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

        public bool LaunchFunction(string functionId, IProcessSimulationContext context, out HRCFunction function, IObservable<HRCProcessResult<HRCFunction>> success = null, IObservable<HRCProcessResult<HRCFunction>> failure = null)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionFailed(HRCFunction function)
        {
            throw new NotImplementedException();
        }

        public void EmitFunctionSucceeded(HRCProcessResult<HRCFunction> result)
        {
            throw new NotImplementedException();
        }

        public void Process(HRCProcess process, IObservable<HRCProcessResult<HRCProcess>> processResult)
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