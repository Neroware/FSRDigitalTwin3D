using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using SimSharp;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class SimSharpProcessSimulation : ProcessSimulationBase
    {
        private CompositeDisposable _disposable;
        private Simulation _environment;
        private Event _stopEvent;
        private IProcessSimulationContext _context;

        public Simulation Environment => _environment;

        public Process Process(IEnumerable<Event> generator, int priority = 0)
            => _environment.Process(generator, priority);

        protected override void OnInitialize(IProcessSimulationContext context)
        {
            _environment = new Simulation();
            _environment.SetVirtualtime();
            _stopEvent = new(_environment);
            _context = context;
            _disposable = new();
            InitTaskHierarchy();
        }

        protected override void OnReset()
        {
            _disposable.Dispose();
            OnInitialize(_context);
        }

        protected override void OnRun()
        {
            _disposable.Add(new NaiveTaskScheduler().Schedule(this, _context));
            // _environment.Run(_stopEvent);
            _disposable.Add(_context.Simulation.ProcessFinished.Subscribe(p => UnityEngine.Debug.Log($"Process finished: {p}")));
        }

        protected override void OnStop()
        {
            _stopEvent.Trigger(_stopEvent);
        }

        private void InitTaskHierarchy()
        {
            foreach (HRCGoal goal in _context.Goals.Keys)
            {
                var sub = _processFinished
                    .Where(p => p.Process.ProcessType == EHRCProcessType.Method
                        && _context.Goals[goal].Contains(p.Process as HRCMethod))
                    .First()
                    .Subscribe(_ => _processFinished.OnNext(new HRCProcessResult() { Process = goal, TimeStamp = _environment.Now }));
                _disposable.Add(sub);

                foreach (HRCMethod method in _context.Goals[goal])
                {
                    var methodTasks = _context.Methods[method].Keys
                        .Where(task => !_context.Methods[method].Values
                            .Any(x => x.Any(x => x.Any(x => x.TaskId == task.TaskId))));
                    var methodTasksFinished = methodTasks
                        .Select(task => _processFinished.Where(t => t.Process.ProcessType >= EHRCProcessType.Task
                            && ((HRCTask)t.Process).TaskId == task.TaskId));
                    sub = Observable.Zip(methodTasksFinished)
                        .First()
                        .Subscribe(_ => _processFinished.OnNext(
                            new HRCProcessResult() { Process = method, TimeStamp = _environment.Now }));
                    _disposable.Add(sub);

                    foreach (var task in _context.Methods[method].Keys)
                    {
                        sub = _context.Methods[method][task].Select(conj => Observable.Zip(conj.Select(task => _processFinished
                            .Where(t => t.Process.ProcessType >= EHRCProcessType.Task
                                && task.TaskId == ((HRCTask)t.Process).TaskId))))
                            .First()
                            .Subscribe(_ => _processFinished.OnNext(
                                new HRCProcessResult() { Process = task, TimeStamp = _environment.Now }));
                        _disposable.Add(sub);
                    }
                }
            }
        }

        protected override void OnProcess(HRCProcess process, IObservable<HRCProcessResult> success, IObservable<Exception> failure)
        {
            Event p = new(_environment);
            IEnumerable<Event> process_()
            {
                _processStarted.OnNext(process);
                yield return p;
                _processFinished.OnNext(new HRCProcessResult()
                {
                    Process = process,
                    TimeStamp = _environment.Now,
                    Outputs = new object[0]
                });
            }
            _disposable.Add(success.Subscribe(_ => { p.Trigger(p); }));
            _disposable.Add(failure.Subscribe(_ => { p.Fail(); _processFailed.OnNext(process); }));
            _environment.Process(process_());
        }

        protected override void OnProcess(HRCFunction function, bool realtime, IObservable<HRCProcessResult> success, IObservable<Exception> failure)
        {
            if (realtime)
            {
                OnProcess(function, success, failure);
            }
            else
            {
                Event p = new(_environment);
                Event timeout_ = _environment.Timeout(function?.FunctionDescription.Duration ?? TimeSpan.Zero);
                IEnumerable<Event> process_()
                {
                    _processStarted.OnNext(function);
                    yield return new AnyOf(_environment, p, timeout_);
                    _processFinished.OnNext(new HRCProcessResult()
                    {
                        Process = function,
                        TimeStamp = _environment.Now,
                        Outputs = new object[0]
                    });
                }
                _disposable.Add(success.Subscribe(_ => { p.Trigger(p); }));
                _disposable.Add(failure.Subscribe(_ => { p.Fail(); _processFailed.OnNext(function); }));
                _environment.Process(process_());
            }
        }

        public override DateTime Now() => _environment.Now;
    }

}