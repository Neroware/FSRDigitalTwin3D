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

        protected override void OnInitialize(IProcessSimulationContext context)
        {
            _environment = new Simulation();
            _environment.SetVirtualtime();
            _stopEvent = new(_environment);
        }

        protected override void OnReset()
        {
            _disposable.Dispose();
            _disposable = null;
        }

        protected override void OnRun()
        {
            _environment.Run();
        }

        protected override void OnStop()
        {
            _stopEvent.Trigger(_stopEvent);
        }
    }

}