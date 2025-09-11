using UnityEngine;

using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.ROS.Robot {

    public abstract class RosSourceDestinationPublisherBase : MonoBehaviour, IRosSourceDestinationPublisher
    {
        public abstract string TopicName { get; }
        public abstract GameObject Robot { get; }
        public abstract GameObject Target { get; }
        public abstract GameObject TargetPlacement { get; }
        public abstract void Publish();
    }

}