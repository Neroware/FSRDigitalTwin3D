using AdminShellNS.Models;

namespace FSR.DigitalTwin.App.Interfaces;

public interface IOperationalState {

    IDictionary<string, ExecutionState> Status { get; init; }
    IDictionary<string, TaskCompletionSource<OperationResult>> Tasks { get; init; }
    IDictionary<string, OperationResult> Results { get; init; }

}