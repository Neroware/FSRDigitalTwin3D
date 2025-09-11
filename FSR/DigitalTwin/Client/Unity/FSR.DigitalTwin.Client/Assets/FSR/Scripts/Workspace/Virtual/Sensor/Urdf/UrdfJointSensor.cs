using System.Linq;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Sensor.Urdf {

    public abstract class UrdfJointSensor : MonoBehaviour, ISensorSource<float[]>
    {
        [SerializeField] protected ArticulationBody _articulationBody;
        [SerializeField] protected string _jointPrefix = "joint_0";
        protected ReactiveCollection<float> _jointOrientation = new(new float[0]);
        public ReadOnlyReactiveProperty<float[]> SensorData => 
            new (_jointOrientation.ObserveReplace().Select(_ => _jointOrientation.ToArray()));

        public float[] Orientation => _jointOrientation.ToArray();
        public string JointName => _jointPrefix;

        public float[] GetOrientation() {
            return _jointOrientation.ToArray();
        }

        protected void Awake() {
            _articulationBody ??= GetComponent<ArticulationBody>();
        }
        
    }

}