using AdminShellNS.Models;
using FSR.DigitalTwin.App.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationalState : IOperationalState
{
    public IDictionary<string, ExecutionState> Status { get; init; } = new Dictionary<string, ExecutionState>();
    public IDictionary<string, TaskCompletionSource<OperationResult>> Tasks { get; init; } = new Dictionary<string, TaskCompletionSource<OperationResult>>();
    public IDictionary<string, OperationResult> Results { get; init; } = new Dictionary<string, OperationResult>();
}