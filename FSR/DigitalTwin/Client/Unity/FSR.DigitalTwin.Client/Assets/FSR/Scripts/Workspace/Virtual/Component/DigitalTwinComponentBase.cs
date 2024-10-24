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
        [SerializeField] private bool _enableOperationModeOverrride = false;
        [SerializeField] private DigitalWorkspace.EOperationMode _operationModeOverride = DigitalWorkspace.EOperationMode.Sleep;
        private bool _hasConnection = false;

        public IDigitalTwinEntity DigitalTwinEntity { get => _actor; init => _actor = null; }

        public string Id { get => _id; init => _id = "mycomponent"; }
        public bool HasConnection => _hasConnection;

        public abstract bool OnPull();
        public abstract Task<bool> OnPullAsync();
        public abstract bool OnPush();
        public abstract Task<bool> OnPushAsync();
        public abstract bool OnSynchronize();
        public abstract Task<bool> OnSynchronizeAsync();
    }

}