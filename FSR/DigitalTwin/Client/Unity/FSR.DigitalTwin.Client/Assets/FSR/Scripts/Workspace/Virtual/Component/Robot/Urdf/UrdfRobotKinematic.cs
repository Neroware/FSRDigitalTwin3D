using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Actor.Robot;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Robot.Urdf;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Robot.Urdf.Joints;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Robot.Urdf {

    public class UrdfRobotKinematic : DigitalTwinComponentBase
    {
        [SerializeField] private List<UrdfJointSensor> _joints;
        [SerializeField] private RosSourceDestinationPublisherBase _rosSourceDestinationPublisher;
        [SerializeField] private float[] _defaultPoseConfiguration = new float[] { -90.0f, -45.0f, 0.0f, -45.0f, -90.0f, 0.0f };

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

        private async Task UpdateJointOrientationsAsync() {
            foreach (UrdfJointSensor joint in _joints) {
                
                switch (joint) {
                    case UrdfRevoluteJointSensor: {
                        float z = await DigitalWorkspace.Instance.Entities.GetComponentPropertyAsync<float>(Id, "Orientation." + joint.JointName + "_z");
                        ArticulationBody articulationBody = joint.GetComponent<ArticulationBody>();
                        articulationBody.SetDriveTarget(ArticulationDriveAxis.X, z);
                    } break;
                    case UrdfFixedJointSensor: {
                        // Intentionally left empty
                    }
                    break;
                }
            }
        }

        private new void Start() {
            base.Start();
            DigitalWorkspace.Instance.Connection.IsConnected
                .Where(x => x).Subscribe(_ => CreateJointProperties()).AddTo(this);
        }

        public void MoveToDefaultPoseConfiguration() {
            float[] target = _defaultPoseConfiguration;
            for (int i = 0; i < 6; i++) {
                ArticulationDrive xDrive = _joints[i + 1].GetComponent<ArticulationBody>().xDrive;
                xDrive.target = target[i];
                _joints[i + 1].GetComponent<ArticulationBody>().xDrive = xDrive;
            }
        }

        public override bool OnPull()
        {
            UpdateJointOrientationsAsync().GetAwaiter().GetResult();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        public override async Task<bool> OnPullAsync()
        {
            await UpdateJointOrientationsAsync();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
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