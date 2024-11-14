using AdminShellNS.Models;
using FSR.DigitalTwin.App.Interfaces;

namespace FSR.DigitalTwin.App;

public class OperationalState : IOperationalState
{
    public IDictionary<string, ExecutionState> ExecutionStates { get; init; } = new Dictionary<string, ExecutionState>();
    public IDictionary<string, TaskCompletionSource<OperationResult>> Results { get; init; } = new Dictionary<string, TaskCompletionSource<OperationResult>>();
}