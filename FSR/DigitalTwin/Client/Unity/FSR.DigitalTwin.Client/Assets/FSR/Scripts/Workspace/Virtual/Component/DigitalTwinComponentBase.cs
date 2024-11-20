using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Actor;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component {

    public abstract class DigitalTwinComponentBase : MonoBehaviour, IDigitalTwinEntityComponent
    {
        [SerializeField] private string _id = "mycomponent";
        [SerializeField] private DigitalTwinActorBase _actor;
        [SerializeField] private bool _enableOperationModeOverride = false;
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

        private DigitalWorkspace.EOperationMode _operationMode => 
            _enableOperationModeOverride ? _operationModeOverride : DigitalWorkspace.Instance.OperationMode;

        protected void Start() {
            // TODO Adjust!
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0))
                .Where(_ => DigitalWorkspace.Instance.Connection.IsConnected.Value)
                .Subscribe(async _ => {
                    switch (_operationMode) {
                        case DigitalWorkspace.EOperationMode.Push: await OnPushAsync(); break;
                        case DigitalWorkspace.EOperationMode.Pull: await OnPullAsync(); break;
                        case DigitalWorkspace.EOperationMode.Sync: await OnSynchronizeAsync(); break;
                    }
                })
                .AddTo(this);
        }
    }

}