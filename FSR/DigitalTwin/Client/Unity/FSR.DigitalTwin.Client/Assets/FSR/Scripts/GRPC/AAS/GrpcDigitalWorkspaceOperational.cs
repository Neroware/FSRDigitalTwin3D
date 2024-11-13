using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.Services.SubmodelService;
using FSR.DigitalTwin.Client.Unity.GRPC.AAS.Utils;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using Grpc.Core;
using Unity.VisualScripting;

namespace FSR.DigitalTwin.Client.Unity.GRPC.AAS {

    public class GrpcDigitalWorkspaceOperational : IDigitalWorkspaceOperational {
        
        private readonly Channel _rpcChannel;
        private readonly GrpcAdminShellApiServiceClient _client;
        private static long _counter = 0;

        public GrpcDigitalWorkspaceOperational(Channel channel) {
            _rpcChannel = channel;
            _client = new(channel);
        }

        public bool GetResult(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> GetResultAsync(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public bool HasSucceeded(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> HasSucceededAsync(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsCompleted(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsCompletedAsync(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public bool IsRunning(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> IsRunningAsync(string actorId, string processId)
        {
            throw new System.NotImplementedException();
        }

        public bool LaunchProcess(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> LaunchProcessAsync(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            throw new System.NotImplementedException();
        }

        public bool RunProcess(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(actorId),
                Timestamp = -1,
                RequestId = "FSR.DigitalTwin.Client.Unity::" + _counter++
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = _client.Submodel.InvokeOperationSync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Payload.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            return true;
        }

        public async Task<bool> RunProcessAsync(string actorId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(actorId),
                Timestamp = -1,
                RequestId = "FSR.DigitalTwin.Client.Unity::" + _counter++
            };

            string[] path = processId.Split('.');
            foreach (string idShort in path) {
                request.Path.Add(new KeyDTO() { Type = KeyTypes.SubmodelElement, Value = idShort });
            }
            request.InputArguments.AddRange(inputVars);
            request.InoutputArguments.AddRange(inOutVars);

            var response = await _client.Submodel.InvokeOperationSyncAsync(request);
            if (response.StatusCode != (int) HttpStatusCode.OK) {
                return false;
            }
            if (!response.Payload.Success) {
                return false;
            }

            inOut.Clear();
            inOut.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            output.AddRange(response.Payload.InoutputArguments.Select(x => x.GetRawValue<object>()));
            return true;
        }
    }

}