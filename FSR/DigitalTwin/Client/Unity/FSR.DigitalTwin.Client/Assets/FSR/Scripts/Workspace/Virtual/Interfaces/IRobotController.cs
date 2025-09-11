using System.Threading.Tasks;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces
{

    /// <summary>
    /// FSRDigitalTwin3D's common interface for all robot controllers, be it a ROS2-controller, 
    /// a game logic in Unity that describes movement or any other thing than can plan and run a movement.
    /// </summary>
    public interface IRobotController
    {
        GameObject Robot { get; }
        bool HasPlanned { get; }
        bool IsValid { get; }
        bool IsInterrupted { get; }
        bool IsRunning { get; }

        public void Plan();
        public bool ValidatePlan();
        public void RunPlan();
        public void PlanAndRunIfValid();
        public bool Interrupt();
        public void ForceInterrupt();
    }

    public interface IRobotAsyncController : IRobotController
    {
        public Task PlanAsync();
        public Task<bool> ValidatePlanAsync();
        public Task RunPlanAsync();
        public Task PlanAndRunIfValidAsync();
        public Task<bool> InterruptAsync();
        public Task ForceInterruptAsync();
    }

}