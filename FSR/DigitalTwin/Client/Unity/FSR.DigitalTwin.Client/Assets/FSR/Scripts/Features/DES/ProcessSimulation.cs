using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
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

    public abstract class ProcessSimulationBase : IProcessSimulation
    {
        public IObservable<IProcessSimulation> SimulationStarted => _simulationStarted;
        public IObservable<IProcessSimulation> SimulationFinished => _simulationFinished;
        public IObservable<IProcessSimulation> SimulationReset => _simulationReset;
        public IObservable<HRCProcess> ProcessStarted => _processStarted;
        public IObservable<HRCProcessResult> ProcessFinished => _processFinished;
        public IObservable<HRCProcess> ProcessFailed => _processFailed;

        protected Subject<IProcessSimulation> _simulationStarted = new();
        protected Subject<IProcessSimulation> _simulationFinished = new();
        protected Subject<IProcessSimulation> _simulationReset = new();
        protected Subject<HRCProcess> _processStarted = new();
        protected Subject<HRCProcessResult> _processFinished = new();
        protected Subject<HRCProcess> _processFailed = new();

        public bool Initialize(out IProcessSimulationContext context)
        {
            try
            {
                context = DigitalWorkspace.Instance.Knowledge.GetContext();
                context.Simulation = this;
                OnInitialize(context);
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError(e.Message);
                context = null;
                return false;
            }
        }

        public void Process(HRCProcess process, IObservable<HRCProcessResult> success_ = null, IObservable<Exception> failure_ = null)
        {
            IObservable<HRCProcessResult> success = success_ ?? Observable.Never<HRCProcessResult>();
            if (process is HRCFunction function)
            {
                OnFunctionLaunch(function, out bool hasOperator, out SocialOperatorBase socialOperator);
                if (hasOperator && socialOperator != null)
                {
                    success = success.Merge(
                        Task.Run(() => (HRCProcessResult)socialOperator.RunFunction(
                            function.FunctionDescription.Name, function.Inputs, function.InOuts))
                                .ToObservable()
                    );
                }
            }
            OnProcess(process, success, failure_);
        }

        public void Reset()
        {
            OnReset();
            _simulationReset.OnNext(this);
        }
        public void Run()
        {
            OnRun();
            _simulationStarted.OnNext(this);
        }

        protected abstract void OnRun();
        protected abstract void OnReset();
        protected virtual void OnFunctionLaunch(HRCFunction function, out bool hasOperator, out SocialOperatorBase socialOperator)
        {
            hasOperator = false;
            socialOperator = null;
        }
        protected abstract void OnInitialize(IProcessSimulationContext context);
        protected virtual void OnProcess(HRCProcess process, IObservable<HRCProcessResult> success, IObservable<Exception> failure) => _processStarted.OnNext(process);
    }

}