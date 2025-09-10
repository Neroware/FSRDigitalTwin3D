using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot;
using UniRx;
using UnityEngine;

public abstract class RobotOperatorBase : DigitalTwinComponentBase, IRobotOperator
{
    public bool IsBusy => throw new System.NotImplementedException();
    public string RunningOperation => throw new System.NotImplementedException();
    private bool _localInvoke = false;
    
    public void Invoke(string process, object[] inputs, object[] inOuts)
    {
        if (!IsBusy)
        {
            ProcessInvocation invocation = new()
            {
                Id = Id,
                OwnerId = DigitalTwinEntity.Id,
                ProcessName = process,
                Inputs = inputs,
                InOuts = inOuts
            };
            OnInvoke(invocation);
        }
    }

    protected abstract void OnInvoke(ProcessInvocation invocation);

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

    protected override bool OnSynchronize()
    {
        throw new System.NotImplementedException();
    }

    protected override Task<bool> OnSynchronizeAsync()
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
