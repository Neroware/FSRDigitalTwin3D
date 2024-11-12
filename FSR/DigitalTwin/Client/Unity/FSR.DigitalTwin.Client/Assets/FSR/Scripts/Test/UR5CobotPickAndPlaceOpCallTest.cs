using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Test {

    public class UR5CobotPickAndPlaceOpCallTest : MonoBehaviour {

        void Start() {
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => x).Subscribe(_ => RunTest());
        }

        private void RunTest() {
            Debug.Log(">>>> " + DigitalWorkspace.Instance.Connection);
        }

    }

}