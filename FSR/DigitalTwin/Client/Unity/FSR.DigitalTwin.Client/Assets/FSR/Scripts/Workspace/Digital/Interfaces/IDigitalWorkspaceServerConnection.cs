using System;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using UniRx;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {

    public interface IDigitalWorkspaceServerConnection : IDisposable {
        ReadOnlyReactiveProperty<bool> IsConnected { get; }
        IObservable<ServerNotificationBase> OnNotify { get; }

        Task<bool> Connect(string[] connArgs = null);
        Task<bool> Disconnect();
    }

}