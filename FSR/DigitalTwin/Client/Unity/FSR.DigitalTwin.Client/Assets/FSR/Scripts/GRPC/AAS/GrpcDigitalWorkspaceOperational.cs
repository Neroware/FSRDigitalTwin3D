using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3.Services.Services.SubmodelService;
using FSR.DigitalTwin.Client.Unity.GRPC.AAS.Utils;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using Grpc.Core;
using UniRx;
using Unity.VisualScripting;

namespace FSR.DigitalTwin.Client.Unity.GRPC.AAS {

    public class GrpcDigitalWorkspaceOperational : IDigitalWorkspaceOperational {
        
        private readonly Channel _rpcChannel;
        private readonly GrpcAdminShellApiServiceClient _client;
        private static long _counter = 0;

        public IObservable<ProcessInvocation> ProcessInvoked => DigitalWorkspace.Instance.Connection.OnNotify
            .Where(x => x.Type == EClientNotificationType.PROCESS_INVOKED)
            .Select(x => (ProcessInvocation) x);

        public GrpcDigitalWorkspaceOperational(Channel channel) {
            _rpcChannel = channel;
            _client = new(channel);
        }

        public bool RunProcess(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
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

        public async Task<bool> RunProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut, IList<object> output)
        {
            var inputVars = input.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));
            var inOutVars = inOut.Select(x => OperationVariableFactory.From(SubmodelElementType.Property, x));

            InvokeOperationSyncRequest request = new() {
                SubmodelId = Base64Converter.ToBase64(ownerId),
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

        public bool LaunchProcess(string ownerId, string processId, IList<object> input, IList<object> inOut)
        {
            throw new NotImplementedException();
        }

        public Task<bool> LaunchProcessAsync(string ownerId, string processId, IList<object> input, IList<object> inOut)
        {
            throw new NotImplementedException();
        }

        public bool GetResult(string ownerId, string processId, IList<object> inOut, IList<object> output)
        {
            throw new NotImplementedException();
        }

        public Task<bool> GetResultAsync(string ownerId, string processId, IList<object> inOut, IList<object> output)
        {
            throw new NotImplementedException();
        }

        public bool IsRunning(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsRunningAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public bool IsCompleted(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> IsCompletedAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public bool HasSucceeded(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasSucceededAsync(string ownerId, string processId)
        {
            throw new NotImplementedException();
        }

        public async void SetResult(ProcessResult result)
        {
            await DigitalWorkspace.Instance.Connection.Notify(result);
        }

        public async Task SetResultAsync(ProcessResult result)
        {
            await DigitalWorkspace.Instance.Connection.Notify(result);
        }
    }

}