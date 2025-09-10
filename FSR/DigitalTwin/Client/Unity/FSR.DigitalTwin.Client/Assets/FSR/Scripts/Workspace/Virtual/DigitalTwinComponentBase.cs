using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual {

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

        protected abstract bool OnPull();
        protected abstract Task<bool> OnPullAsync();
        protected abstract bool OnPush();
        protected abstract Task<bool> OnPushAsync();
        protected abstract bool OnSynchronize();
        protected abstract Task<bool> OnSynchronizeAsync();

        private DigitalWorkspace.EOperationMode _operationMode => 
            _enableOperationModeOverride ? _operationModeOverride : DigitalWorkspace.Instance.OperationMode;

        protected void Start() {
            // TODO Adjust!
            bool requestRunning = false;
            Observable.Timer(TimeSpan.Zero, TimeSpan.FromSeconds(1.0f))
                .Where(_ => DigitalWorkspace.Instance.Connection.IsConnected.Value)
                .Subscribe(async _ => {
                    if (requestRunning) return;
                    requestRunning = true;
                    switch (_operationMode) {
                        case DigitalWorkspace.EOperationMode.Push: await OnPushAsync(); break;
                        case DigitalWorkspace.EOperationMode.Pull: await OnPullAsync(); break;
                        case DigitalWorkspace.EOperationMode.Sync: await OnSynchronizeAsync(); break;
                    }
                    requestRunning = false;
                })
                .AddTo(this);
        }
    }

}