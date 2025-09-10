using System.Threading.Tasks;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {

    public interface IDigitalTwinEntityComponent {
        IDigitalTwinEntity DigitalTwinEntity { get; init; }
        string Id { get; init; }
        bool HasConnection { get; }
    }

}