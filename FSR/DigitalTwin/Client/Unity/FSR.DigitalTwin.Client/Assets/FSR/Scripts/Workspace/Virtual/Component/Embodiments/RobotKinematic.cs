using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Urdf;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.ROS.Robot;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Robot.Component.Embodiments {

    public class RobotKinematic : DigitalTwinComponentBase
    {
        [SerializeField] private List<UrdfJointSensor> _joints;
        [SerializeField] private RosSourceDestinationPublisherBase _rosSourceDestinationPublisher;
        [SerializeField] private float[] _defaultPoseConfiguration = new float[] { -90.0f, -45.0f, 0.0f, -45.0f, -90.0f, 0.0f };

        private async Task UpdateJointPropertiesAsync() {
            string path = "Segments.";
            foreach (UrdfJointSensor joint in _joints) {
                path += joint.name + ".";
                switch(joint) {
                    case UrdfRevoluteJointSensor: {
                        await DigitalWorkspace.Instance.Entities.SetComponentPropertyAsync(Id, path + "theta", joint.Orientation[0]);
                    } break;
                    case UrdfFixedJointSensor: {
                        // Intentionally left empty
                    } break;
                }
                path += "Children.";
            }
        }

        private async Task UpdateJointOrientationsAsync() {
            string path = "Segments.";
            foreach (UrdfJointSensor joint in _joints) {
                path += joint.name + ".";
                switch (joint) {
                    case UrdfRevoluteJointSensor: {
                        float z = await DigitalWorkspace.Instance.Entities.GetComponentPropertyAsync<float>(Id, path + "theta");
                        ArticulationBody articulationBody = joint.GetComponent<ArticulationBody>();
                        articulationBody.SetDriveTarget(ArticulationDriveAxis.X, z);
                    } break;
                    case UrdfFixedJointSensor: {
                        // Intentionally left empty
                    }
                    break;
                }
                path += "Children.";
            }
        }

        public void MoveToDefaultPoseConfiguration() {
            float[] target = _defaultPoseConfiguration;
            for (int i = 0; i < 6; i++) {
                ArticulationDrive xDrive = _joints[i + 1].GetComponent<ArticulationBody>().xDrive;
                xDrive.target = target[i];
                _joints[i + 1].GetComponent<ArticulationBody>().xDrive = xDrive;
            }
        }

        protected override bool OnPull()
        {
            UpdateJointOrientationsAsync().GetAwaiter().GetResult();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        protected override async Task<bool> OnPullAsync()
        {
            await UpdateJointOrientationsAsync();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        protected override bool OnPush()
        {
            UpdateJointPropertiesAsync().GetAwaiter().GetResult();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }

        protected override async Task<bool> OnPushAsync()
        {
            await UpdateJointPropertiesAsync();
            return DigitalWorkspace.Instance.Connection.IsConnected.Value;
        }
    }

}