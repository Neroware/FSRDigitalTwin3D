using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Robots.Urdf.Joints;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Component.Robots.Urdf {

    public class UrdfRobotKinematic : DigitalTwinComponentBase
    {
        [SerializeField] private List<UrdfJoint> _joints;

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
            throw new System.NotImplementedException();
        }

        public override Task<bool> OnPushAsync()
        {
            throw new System.NotImplementedException();
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