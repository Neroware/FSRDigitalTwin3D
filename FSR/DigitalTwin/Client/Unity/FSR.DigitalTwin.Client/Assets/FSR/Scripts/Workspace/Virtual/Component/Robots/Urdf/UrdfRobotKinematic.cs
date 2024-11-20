using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Robots.Urdf;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Robots.Urdf.Joints;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Robots.Urdf {

    public class UrdfRobotKinematic : DigitalTwinComponentBase
    {
        [SerializeField] private List<UrdfJointSensor> _joints;


        private void CreateJointProperties() {
            foreach (UrdfJointSensor joint in _joints) {
                
                switch (joint) {
                    case UrdfRevoluteJointSensor: {
                        DigitalWorkspace.Instance.Entities.CreateComponentProperty(Id, "Orientation." + joint.JointName + "_z", joint.Orientation[0]);
                    } break;
                    case UrdfFixedJointSensor: {
                        // Intentionally left empty
                    }
                    break;
                }
            }
        }

        private async Task UpdateJointPropertiesAsync() {
            foreach (UrdfJointSensor joint in _joints) {
                
                switch (joint) {
                    case UrdfRevoluteJointSensor: {
                        await DigitalWorkspace.Instance.Entities.SetComponentPropertyAsync(Id, "Orientation." + joint.JointName + "_z", joint.Orientation[0]);
                    } break;
                    case UrdfFixedJointSensor: {
                        // Intentionally left empty
                    }
                    break;
                }
            }
        }

        private void Start() {
            base.Start();
            DigitalWorkspace.Instance.Connection.IsConnected
                .Where(x => x).Subscribe(_ => CreateJointProperties()).AddTo(this);
        }

        public override bool OnPull()
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> OnPullAsync()
        {
            throw new System.NotImplementedException();
        }

        public override bool OnPush()
        {
            UpdateJointPropertiesAsync().GetAwaiter().GetResult();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        public override async Task<bool> OnPushAsync()
        {
            await UpdateJointPropertiesAsync();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        public override bool OnSynchronize()
        {
            throw new System.NotImplementedException();
        }

        public override Task<bool> OnSynchronizeAsync()
        {
            throw new System.NotImplementedException();
        }
    }

}