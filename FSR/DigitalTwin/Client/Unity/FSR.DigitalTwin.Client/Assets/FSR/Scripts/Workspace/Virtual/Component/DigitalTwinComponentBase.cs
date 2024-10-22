using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Actor;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component {

    public abstract class DigitalTwinComponentBase : MonoBehaviour, IDigitalTwinEntityComponent
    {
        [SerializeField] private string _id = "mycomponent";
        [SerializeField] private DigitalTwinActorBase _actor;
        private bool _hasConnection = false;

        public IDigitalTwinEntity DigitalTwinEntity { get => _actor; init => _actor = null; }

        public string Id { get => _id; init => _id = "mycomponent"; }
        public bool HasConnection => _hasConnection;

        public T GetProperty<T>(string prop)
        {
            return DigitalWorkspace.Instance.Entities.GetComponentProperty<T>(_id, prop);
        }

        public async Task<T> GetPropertyAsync<T>(string prop)
        {
            return await DigitalWorkspace.Instance.Entities.GetComponentPropertyAsync<T>(_id, prop);
        }

        public bool SetProperty<T>(string prop, T value)
        {
            return DigitalWorkspace.Instance.Entities.SetComponentProperty(_id, prop, value);
        }

        public async Task<bool> SetPropertyAsync<T>(string prop, T value)
        {
            return await DigitalWorkspace.Instance.Entities.SetComponentPropertyAsync(_id, prop, value);
        }
    }

}