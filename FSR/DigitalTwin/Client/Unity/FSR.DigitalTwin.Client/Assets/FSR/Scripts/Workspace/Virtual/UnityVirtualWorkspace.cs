using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual
{
    public class UnityVirtualWorkspace : MonoBehaviour, IVirtualWorkspace
    {
        [SerializeField] private SimulationManager simulationManager;
        public IProcessSimulation ProcessSimulation => simulationManager.ActiveScenario;

        public UnityVirtualWorkspace()
        {
            VirtualWorkspace.SetWorkspace(this);
        }
    }
}