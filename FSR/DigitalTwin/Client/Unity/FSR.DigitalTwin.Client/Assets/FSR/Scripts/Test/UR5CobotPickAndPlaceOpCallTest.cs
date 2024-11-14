using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.GRPC.AAS;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using UniRx;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Test {

    public class UR5CobotPickAndPlaceOpCallTest : MonoBehaviour {

        void Start() {
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => x).Subscribe(_ => RunTest()).AddTo(this);
            DigitalWorkspace.Instance.Connection.IsConnected.Where(x => x).Subscribe(_ => {
                DigitalWorkspace.Instance.Operational.ProcessInvoked.Subscribe(x => RunDummyProcess(x)).AddTo(this);
            }).AddTo(this);
        }

        private async void RunTest() {
            Debug.Log(">>>> " + DigitalWorkspace.Instance.Connection);

            List<object> inputs = new() { 42, 43, 44, 45 };
            List<object> inOuts = new() { 4242 };
            List<object> outputs = new() { };
            var result = await DigitalWorkspace.Instance.Operational.RunProcessAsync("https://www.hs-emden-leer.de/ids/sm/6494_2162_5032_2813", "pick_and_place", inputs, inOuts, outputs);
            
            Debug.Log(">>>> " + result);
        }

        private async void RunDummyProcess(ProcessInvocation invocation) {
            await Task.Delay(2000);
            await DigitalWorkspace.Instance.Operational.SetResultAsync(new ProcessResult() {
                Id = invocation.Id,
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                OwnerId = invocation.OwnerId,
                ProcessName = invocation.ProcessName,
                InOuts = new object[0],
                Outputs = new object[] { true },
                TimeStamp = -1
            });
        }

    }

}