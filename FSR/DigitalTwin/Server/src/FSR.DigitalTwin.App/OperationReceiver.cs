using AasCore.Aas3_0;
using AdminShellNS;
using AdminShellNS.Models;
using FSR.DigitalTwin.App.Interfaces.Services;

namespace FSR.DigitalTwin.App;

public class OperationReceiver : IOperationReceiver
{
    private readonly IDigitalTwinOperationalService _operationalService;
    private readonly Dictionary<string, OperationResult> _results = [];

    public OperationReceiver(IDigitalTwinOperationalService operationalService) {
        _operationalService = operationalService ?? throw new NullReferenceException();
    }

    public OperationResult GetResult(string handleId)
    {
        var result = _results[handleId];
        _results.Remove(handleId);
        return result;
    }

    public OperationResult OnOperationInvoke(IOperation operation, string submodelId, int? timestamp, string requestId)
    {
        _operationalService.InvokeAsync(operation, timestamp, requestId, submodelId);
        return _operationalService.GetResultAsync(requestId).Result;
    }

    public async Task<OperationResult> OnOperationInvokeAsync(string handleId, IOperation operation, string submodelId, int? timestamp, string requestId)
    {
        // Note: We currently assume async operations to not fail. Timestamps have no meanings yet as well!
        await _operationalService.InvokeAsync(operation, timestamp, requestId, submodelId, handleId);
        _operationalService.UpdateExecutionState(handleId, ExecutionState.RunningEnum);
        var result = await _operationalService.GetResultAsync(requestId);
        _results[handleId] = result;
        _operationalService.UpdateExecutionState(handleId, ExecutionState.CompletedEnum);
        return result;
    }
}