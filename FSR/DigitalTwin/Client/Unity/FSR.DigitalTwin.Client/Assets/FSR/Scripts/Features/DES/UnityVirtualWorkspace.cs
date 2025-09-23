using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.DES
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