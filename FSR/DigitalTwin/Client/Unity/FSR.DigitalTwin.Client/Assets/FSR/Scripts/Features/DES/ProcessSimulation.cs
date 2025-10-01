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
        public IObservable<HRCProcessResult<HRCProcess>> ProcessFinished => _processFinished;
        public IObservable<HRCProcess> ProcessFailed => _processFailed;

        protected Subject<IProcessSimulation> _simulationStarted = new();
        protected Subject<IProcessSimulation> _simulationFinished = new();
        protected Subject<IProcessSimulation> _simulationReset = new();
        protected Subject<HRCProcess> _processStarted = new();
        protected Subject<HRCProcessResult<HRCProcess>> _processFinished = new();
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

        public void Process(HRCProcess process, IProcessSimulationContext context, IObservable<HRCProcessResult<HRCProcess>> success_ = null, IObservable<Exception> failure_ = null)
        {
            IObservable<HRCProcessResult<HRCProcess>> success = success_ ?? Observable.Never<HRCProcessResult<HRCProcess>>();
            if (process is HRCFunction function)
            {
                OnFunctionLaunch(function, out bool hasOperator, out SocialOperatorBase socialOperator);
                if (hasOperator && socialOperator != null) {
                    success = success.Merge(
                        Task.Run(socialOperator.RunFunction(function.FunctionDescription.Name, function.Inputs, function.InOuts))
                    );
                }
                
            }
        }

        // public virtual bool LaunchFunction(string functionId, IProcessSimulationContext context, out HRCFunction function, IObservable<HRCProcessResult<HRCFunction>> success = null, IObservable<HRCProcessResult<HRCFunction>> failure = null)
        // {
        //     throw new NotImplementedException();
        // }
        // public virtual void EmitFunctionFailed(HRCFunction function) => _processFailed.OnNext(function);
        // public virtual void EmitFunctionSucceeded(HRCProcessResult<HRCFunction> result) => _processFinished.OnNext(new HRCProcessResult<HRCProcess>()
        // {
        //     Process = result.Process,
        //     Outputs = result.Outputs,
        //     TimeStamp = result.TimeStamp
        // });

        public abstract void Reset();
        public abstract void Run();

        protected virtual void OnFunctionLaunch(HRCFunction function, out bool hasOperator, out SocialOperatorBase socialOperator)
        {
            hasOperator = false;
            socialOperator = null;
        }
        protected virtual void OnInitialize(IProcessSimulationContext context) { }
        protected virtual void OnProcess(HRCProcess process, IObservable<HRCProcessResult<HRCProcess>> success, IObservable<Exception> failure) => _processStarted.OnNext(process); 
    }

}