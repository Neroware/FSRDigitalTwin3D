namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Robots.Urdf.Joints {

    public class UrdfRevoluteJointSensor : UrdfJointSensor {

        private new void Awake() {
            base.Awake();
            _jointOrientation = new(new float[1] { _articulationBody.xDrive.target });
        }

        private void Update() {
            _jointOrientation[0] = _articulationBody.xDrive.target;
        }

    }

}