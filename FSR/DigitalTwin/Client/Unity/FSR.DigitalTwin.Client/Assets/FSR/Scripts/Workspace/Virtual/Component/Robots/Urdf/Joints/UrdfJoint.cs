using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Sensors;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Robots.Urdf.Joints {

    public abstract class UrdfJoint : SensorSourceBase<float[]>
    {
        [SerializeField] private ArticulationBody _articulationBody;
        [SerializeField] private string _jointPrefix = "joint_0";

        private ReactiveCollection<float> _orientation = new();
        
        public float[] GetOrientation() {
            return _orientation.ToArray();
        }

        void Awake() {
            _articulationBody ??= GetComponent<ArticulationBody>();
            // TODO Init sensor source
        }

        // Synchronization for Urdf Joints is handled via the UrdfRobotKinematic
        public override bool OnPull() => true;
        public override Task<bool> OnPullAsync() => Task.FromResult(true);
        public override bool OnPush() => true;
        public override Task<bool> OnPushAsync() => Task.FromResult(true);
        public override bool OnSynchronize() => true;
        public override Task<bool> OnSynchronizeAsync() => Task.FromResult(true);
    }

}