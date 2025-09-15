using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;


namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces {

    /// <summary>
    /// The digitized workspace maintained by the digital twin.
    /// 
    /// This interface shall allow access to the following aspects:<br />
    /// - Connection layer services<br />
    /// - Operational and process signals and callbacks<br />
    /// - The underlying semantic information for entities and their components<br />
    /// </summary>
    public interface IDigitalWorkspace
    {
        IDigitalWorkspaceServerConnection Connection { get; }
        IDigitalWorkspaceOperational Operational { get; }
        IDigitalWorkspaceEntityApi Entities { get; }

        DigitalWorkspace.EOperationMode OperationMode { set; get; }
        string WorkspaceName { get; }
    }

}
