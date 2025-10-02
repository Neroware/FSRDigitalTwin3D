using System;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;

namespace FSR.DigitalTwin.Client.Features.DES.SimSharpBridge
{
    public static class TaskScheduler
    {
        public static IDisposable Schedule(ProcessSimulationBase sim, IProcessSimulationContext ctxt)
        {
            // TODO Schedule HRCFunctions with based on InteractionModality
            //
            // We already did the horizontal dependency of tasks and subtasks, now, we do the vertical one 
            // using this scheduler class! We do this by sequentially calling ProcessSimulation.Process(...)
            // each time the previous process terminated using the observables from ProcessSimulationBase.
            
            return null;
        }
    }
}