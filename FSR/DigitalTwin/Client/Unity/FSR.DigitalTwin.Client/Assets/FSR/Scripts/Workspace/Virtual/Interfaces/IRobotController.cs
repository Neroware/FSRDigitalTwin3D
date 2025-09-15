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

        void Plan();
        bool ValidatePlan();
        void RunPlan();
        void PlanAndRunIfValid();
        bool Interrupt();
        void ForceInterrupt();
    }

    public interface IRobotAsyncController : IRobotController
    {
        Task PlanAsync();
        Task<bool> ValidatePlanAsync();
        Task RunPlanAsync();
        Task PlanAndRunIfValidAsync();
        Task<bool> InterruptAsync();
        Task ForceInterruptAsync();
    }

}