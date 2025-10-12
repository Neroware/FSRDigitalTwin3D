

using System;

namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{
    public interface ITaskScheduler
    {
        IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt);
    }
}