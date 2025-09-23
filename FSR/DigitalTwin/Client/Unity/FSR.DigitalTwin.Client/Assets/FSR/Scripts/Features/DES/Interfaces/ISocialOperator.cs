using System;

namespace FSR.DigitalTwin.Client.Features.DES.Interfaces
{

    public interface ISocialOperator
    {
        bool IsBusy { get; }
        string RunningOperation { get; }
        Uri OperatorId { get; }
    }

}