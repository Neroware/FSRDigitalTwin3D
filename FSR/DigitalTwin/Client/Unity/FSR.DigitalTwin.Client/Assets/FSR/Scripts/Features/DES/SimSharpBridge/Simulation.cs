using System;
using System.Threading;
using System.Threading.Tasks;
using SimSharp;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class Simulation : PseudoRealtimeSimulation
    {
        private object _timeLocker = new();
        public override DateTime Now
        {
            get
            {
                lock (_timeLocker)
                {
                    if (!IsRunningInRealtime) return base.Now;
                    return base.Now + TimeSpan.FromMilliseconds(_rtDelayTime.Elapsed.TotalMilliseconds * RealtimeScale.Value);

                }
            }
            protected set => base.Now = value;
        }

        protected new Stopwatch _rtDelayTime = new();

        public Simulation() : this(new DateTime(1970, 1, 1)) { }
        public Simulation(TimeSpan? defaultStep) : this(new DateTime(1970, 1, 1), defaultStep) { }
        public Simulation(DateTime initialDateTime, TimeSpan? defaultStep = null) : this(new PcgRandom(), initialDateTime, defaultStep) { }
        public Simulation(int randomSeed, TimeSpan? defaultStep = null) : this(new DateTime(1970, 1, 1), randomSeed, defaultStep) { }
        public Simulation(DateTime initialDateTime, int randomSeed, TimeSpan? defaultStep = null) : this(new PcgRandom(randomSeed), initialDateTime, defaultStep) { }
        public Simulation(IRandom random, DateTime initialDateTime, TimeSpan? defaultStep = null) : base(random, initialDateTime, defaultStep) { }

        public override object Run(Event stopEvent = null)
        {
            _stop = new CancellationTokenSource();
            if (stopEvent != null) {
                if (stopEvent.IsProcessed) {
                    return stopEvent.Value;
                }
                stopEvent.AddCallback(StopSimulation);
            }
            OnRunStarted();

            var stop = Observable.EveryUpdate()
                .Where(_ => ScheduleQ.Count == 0 || _stop.IsCancellationRequested)
                .First();
            var step = Observable.EveryUpdate().TakeUntil(stop);
            
            void on_simulation_stop()
            {
                // TODO continue thought here...
            }
            
            return new Task<object>(() =>
            {
                stop.Wait();
                return stopEvent.Value;
            });
        }

        public override void Step()
        {
            base.Step();
            
        }

    }
};