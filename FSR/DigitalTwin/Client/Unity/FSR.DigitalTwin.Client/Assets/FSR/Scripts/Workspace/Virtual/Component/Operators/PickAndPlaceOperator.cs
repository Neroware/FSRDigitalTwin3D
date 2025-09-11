using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot;

public class PickAndPlaceOperator : RobotOperatorBase, IRobotOperator
{
    private bool _isBusy = false;
    private string _runningOperation = "idle";

    public override bool IsBusy => _isBusy;
    public override string RunningOperation => _runningOperation;

    protected override FunctionResult OnFunction(string function, IDigitalWorkspaceOperational operatorInst, ProcessExecutionState state, ProcessResult result)
    {
        throw new System.NotImplementedException();
    }
    protected override FunctionResult OnFunction(string function, object[] inputs, object[] inOuts)
    {
        throw new System.NotImplementedException();
    }

    protected override bool OnPull()
    {
        _isBusy = DigitalWorkspace.Instance.Entities.GetComponentProperty<bool>(Id, "is_busy");
        return true;
    }
    protected override async Task<bool> OnPullAsync()
    {
        _isBusy = await DigitalWorkspace.Instance.Entities.GetComponentPropertyAsync<bool>(Id, "is_busy");
        return true;
    }
    protected override bool OnPush()
    {
        return DigitalWorkspace.Instance.Entities.SetComponentProperty(Id, "is_busy", IsBusy);
    }

    protected override async Task<bool> OnPushAsync()
    {
        return await DigitalWorkspace.Instance.Entities.SetComponentPropertyAsync(Id, "is_busy", IsBusy);
    }
}
