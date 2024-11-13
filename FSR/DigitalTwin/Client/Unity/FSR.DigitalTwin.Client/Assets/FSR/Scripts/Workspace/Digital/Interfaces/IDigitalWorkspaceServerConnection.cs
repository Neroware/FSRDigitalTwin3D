using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core.Process;
using UniRx;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {

    public interface IDigitalWorkspaceServerConnection : IDisposable {
        ReadOnlyReactiveProperty<bool> IsConnected { get; }
        // IObservable<ProcessInvocation> ProcessInvoked { get; init; }

        Task<bool> Connect(string[] connArgs = null);
        Task<bool> Disconnect();
    }

}