using System;
using System.Linq;
using System.Threading.Tasks;
using FSR.GRPC.Lib.V3;
using FSR.GRPC.Lib.V3.Services;
using FSR.GRPC.Lib.V3.Services.AssetAdministrationShellRepository;
using FSR.GRPC.Lib.V3.Services.SubmodelRepository;
using FSR.GRPC.Lib.V3.Services.SubmodelService;
using FSR.Workspace.Digital.AAS;
using Grpc.Core;
using UnityEngine;

namespace FSR.Workspace.Digital {

public class DigitalWorkspaceBridge : MonoBehaviour
{
    [SerializeField] private string digitalWorkspaceAddr = "127.0.0.1";
    [SerializeField] private int digitalWorkspacePort = 5001;

    public Channel RpcChannel { get; }
    public AdminShellApiServiceClient AasApiClient { get; }

    public DigitalWorkspaceBridge() {
        RpcChannel = new Channel(digitalWorkspaceAddr, digitalWorkspacePort, ChannelCredentials.Insecure);
        AasApiClient = new AdminShellApiServiceClient(RpcChannel);
    }


    // =========== EXAMPLE REQUESTS =========== //
    async void Start() {
        await RunExampleRequests__(RpcChannel);
    }

    private async Task RunExampleRequests__(Channel channel) {
        OutputModifier defaultOutput = new OutputModifier() {
            Limit = 1000,
            Extent = OutputExtent.WithoutBlobValue,
            Content = OutputContent.Normal,
            Level = OutputLevel.Deep
        };

        GetAllAssetAdministrationShellsRpcRequest request1 = new() {
            OutputModifier = defaultOutput
        };
        var shells = await AasApiClient.AdminShellRepo.GetAllAssetAdministrationShellsAsync(request1);
        foreach(var aas in shells.Payload) {
            Debug.Log("Found AAS with unique id: " + aas.Id);
        }

        GetAllSubmodelsRpcRequest request2 = new() {
            OutputModifier = defaultOutput
        };
        var submodels = await AasApiClient.SubmodelRepo.GetAllSubmodelsAsync(request2);
        foreach(var submodel in submodels.Payload) {
            Debug.Log("Found Submodel with unique id: " + submodel.Id);
        }

        GetAllSubmodelElementsRpcRequest request3 = new() {
            SubmodelId = "aHR0cHM6Ly93d3cuaHMtZW1kZW4tbGVlci5kZS9pZHMvc20vMTExMF8zMTUwXzYwMzJfNzU4Mw",
            OutputModifier = defaultOutput
        };
        var elements = await AasApiClient.Submodel.GetAllSubmodelElementsAsync(request3);
        foreach(var elem in elements.Payload) {
            Debug.Log("Found SubmodelElement with short id: " + elem.IdShort);
        }

        PostSubmodelElementRpcRequest request4 = new() {
            SubmodelId = "aHR0cHM6Ly93d3cuaHMtZW1kZW4tbGVlci5kZS9pZHMvc20vMTExMF8zMTUwXzYwMzJfNzU4Mw",
        };
        request4.SubmodelElement = new SubmodelElementDTO
        {
            IdShort = "MyRange",
            SubmodelElementType = SubmodelElementType.Range,
            Range = new RangePayloadDTO
            {
                ValueType = DataTypeDefXsd.Float,
                Min = "0.0",
                Max = "1.0"
            }
        };
        var postResponse = await AasApiClient.Submodel.PostSubmodelElementAsync(request4);
        if (postResponse.StatusCode == 201) {
            Debug.Log("Posted Submodel Element: " + postResponse.SubmodelElement.IdShort + " with type " + postResponse.SubmodelElement.SubmodelElementType);
        }
        else {
            Debug.Log("Failed posting submodel element with status = " + postResponse.StatusCode);
        }

        // =========== END Example requests =========== //
    }

}

} // END namespace FSR.Workspace.Digital