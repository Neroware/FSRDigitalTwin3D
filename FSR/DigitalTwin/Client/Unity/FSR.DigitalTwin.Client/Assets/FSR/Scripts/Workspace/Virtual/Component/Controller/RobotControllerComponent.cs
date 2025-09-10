using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Controller
{

    public abstract class RobotControllerComponent : DigitalTwinComponentBase, IRobotController
    {
        public abstract GameObject Robot { get; }
        public abstract bool HasPlanned { get; }
        public abstract bool IsValid { get; }
        public abstract bool IsInterrupted { get; }
        public abstract bool IsRunning { get; }

        public abstract void ForceInterrupt();
        public abstract bool Interrupt();

        // TODO Exchange control information with the Digital Twin's shell...

        public override bool OnPull()
        {
            return true;
        }

        public override Task<bool> OnPullAsync()
        {
            return Task.FromResult(true);
        }

        public override bool OnPush()
        {
            return true;
        }

        public override Task<bool> OnPushAsync()
        {
             return Task.FromResult(true);
        }

        public override bool OnSynchronize()
        {
            return true;
        }

        public override Task<bool> OnSynchronizeAsync()
        {
            return Task.FromResult(true);
        }

        public abstract void Plan();
        public abstract void PlanAndRunIfValid();
        public abstract void RunPlan();
        public abstract bool ValidatePlan();
    }

}