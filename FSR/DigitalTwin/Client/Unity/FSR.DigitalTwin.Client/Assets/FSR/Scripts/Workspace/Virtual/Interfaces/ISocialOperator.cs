using System;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces
{

    public interface ISocialOperator
    {
        bool IsBusy { get; }
        string RunningOperation { get; }
        Uri OperatorId { get; }
    }

}