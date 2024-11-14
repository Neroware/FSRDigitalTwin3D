using AdminShellNS.Models;

namespace FSR.DigitalTwin.App.Interfaces;

public interface IOperationalState {

    IDictionary<string, ExecutionState> ExecutionStates { get; init; }
    IDictionary<string, TaskCompletionSource<OperationResult>> Results { get; init; }

}