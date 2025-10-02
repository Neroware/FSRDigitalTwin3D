using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UniRx;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public class SimSharpProcessSimulation : ProcessSimulationBase
    {
        private CompositeDisposable _disposable;
        private Simulation _environment;

        protected override void OnInitialize(IProcessSimulationContext context)
        {
            throw new System.NotImplementedException();
        }

        protected override void OnReset()
        {
            throw new System.NotImplementedException();
        }

        protected override void OnRun()
        {
            throw new System.NotImplementedException();
        }
    }

}