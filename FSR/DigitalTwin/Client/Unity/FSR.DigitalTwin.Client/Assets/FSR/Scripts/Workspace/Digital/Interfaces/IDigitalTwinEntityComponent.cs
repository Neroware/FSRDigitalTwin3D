using System;
using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {

    public interface IDigitalTwinEntityComponent {
        IDigitalTwinEntity DigitalTwinEntity { get; init; }
        Uri Id { get; init; }
        bool HasConnection { get; }
    }

}