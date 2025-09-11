using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot;
using UnityEngine;

public class PickAndPlaceOperator : DigitalTwinComponentBase, IRobotOperator
{
    private bool _isBusy = false;
    private string _runningOperation = "idle";

    public bool IsBusy => _isBusy;
    public string RunningOperation => _runningOperation;

    protected override bool OnPull()
    {
        throw new System.NotImplementedException();
    }

    protected override Task<bool> OnPullAsync()
    {
        throw new System.NotImplementedException();
    }

    protected override bool OnPush()
    {
        throw new System.NotImplementedException();
    }

    protected override Task<bool> OnPushAsync()
    {
        throw new System.NotImplementedException();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
