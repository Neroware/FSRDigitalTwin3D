using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Test {

    public class UR5CobotPickAndPlaceOpCallTest : MonoBehaviour {

        void Start() {
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => x).Subscribe(_ => RunTest());
        }

        private async void RunTest() {
            Debug.Log(">>>> " + DigitalWorkspace.Instance.Connection);

            List<object> inputs = new() { 42 };
            List<object> inOuts = new() { 4242 };
            List<object> outputs = new() { };
            var result = await DigitalWorkspace.Instance.Operational.RunProcessAsync("https://www.hs-emden-leer.de/ids/sm/6494_2162_5032_2813", "pick_and_place", inputs, inOuts, outputs);
            Debug.Log(">>>> " + result);
        }

    }

}