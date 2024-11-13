using AasCore.Aas3_0;
using AasxServerStandardBib.Logging;
using AutoMapper;
using FSR.DigitalTwin.App.GRPC.Aas.Lib.V3;
using FSR.DigitalTwin.App.GRPC.Services.DigitalTwinClientConnectionService;
using FSR.DigitalTwin.App.Interfaces.Services;
using Grpc.Core;
using ExecutionState = AdminShellNS.Models.ExecutionState;
using OperationResult = AdminShellNS.Models.OperationResult;

namespace FSR.DigitalTwin.App.GRPC.Services;

public class DigitalTwinOperationalService : IDigitalTwinOperationalService
{
    private readonly IAppLogger<DigitalTwinOperationalService> _logger;
    private readonly IMapper _mapper;
    private readonly IDigitalTwinClientConnectionService _connectionService;

    private readonly Dictionary<string, ExecutionState> _executionStates = [];
    private readonly Dictionary<string, TaskCompletionSource<OperationResult>> _results = [];

    public DigitalTwinOperationalService(IAppLogger<DigitalTwinOperationalService> logger, IMapper mapper, IDigitalTwinClientConnectionService connectionService) {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        _connectionService = connectionService ?? throw new NullReferenceException(nameof(connectionService));
    }

    public Task<ExecutionState> GetExecutionStateAsync(string handleId)
    {
        return Task.FromResult(_executionStates[handleId]);
    }

    public async Task<OperationResult> GetResultAsync(string requestId)
    {
        var result = await _results[requestId].Task;
        _results.Remove(requestId);
        return result;
    }

    public async Task<ExecutionState> InvokeAsync(IOperation operation, int? timestamp, string requestId, string submodelId, string? handleId = null)
    {
        InvokeOperationClientNotification invocation = new() {
            RequestId = requestId,
            SubmodelId = submodelId,
            OperationIdShort = operation.IdShort,
            Timestamp = timestamp ?? -1,
            IsAsync = handleId != null,
            HandleId = handleId ?? ""
        };
        invocation.InputVariables.AddRange(operation.InputVariables?.Select(x => _mapper.Map<OperationVariableDTO>(x)));
        invocation.InoutVariables.AddRange(operation.InoutputVariables?.Select(x => _mapper.Map<OperationVariableDTO>(x)));
        var connections = _connectionService.GetAllConnections();
        if (handleId != null) {
            _executionStates[handleId] = ExecutionState.InitiatedEnum;
        }
        foreach (var conn in connections) {
            try {
                IAsyncStreamWriter<ClientNotification> writer = (IAsyncStreamWriter<ClientNotification>) conn.Item2;
                _results[requestId] = new();
                await writer.WriteAsync(new ClientNotification() { Type = ClientNotificationType.InvokeOperation, InvokeOperation = invocation });
            } catch (ObjectDisposedException) { 
                _logger.LogError("Invocation failed because the underlying rpc stream was disposed!");
            }
        }
        return ExecutionState.InitiatedEnum;
    }

    public void UpdateExecutionState(string handleId, ExecutionState executionState)
    {
        _executionStates[handleId] = executionState;
    }

    public void SetResult(string requestId, OperationResult result) 
    {
        _results[requestId].SetResult(result);
    }
}