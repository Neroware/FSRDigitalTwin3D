using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Unity.GRPC.AAS;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Core;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Notification;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces.Robot;
using UniRx;
using UnityEngine;

public abstract class RobotOperatorBase : DigitalTwinComponentBase, IRobotOperator
{
    public abstract bool IsBusy { get; }
    public abstract string RunningOperation { get; }
    public record FunctionResult
    {
        public object[] InOuts { init; get; }
        public object[] Outputs { init; get; }
        public long TimeStamp { init; get; }
    }

    protected abstract FunctionResult OnFunction(string function, IDigitalWorkspaceOperational operatorInst, ProcessExecutionState state, ProcessResult result);
    protected abstract FunctionResult OnFunction(string function, object[] inputs, object[] inOuts);

    protected override void OnConnect()
    {
        DigitalWorkspace.Instance.Operational.ProcessInvoked
            .Where(i => i.OwnerId == Id)
            .Subscribe(RunFunction).AddTo(this);
    }

    private async void RunFunction(ProcessInvocation invocation)
    {
        if (IsBusy)
        {
            throw new InvalidOperationException("Cannot run function because operator is busy!");
        }
        ProcessExecutionState state = new()
        {
            ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
            Id = invocation.Id,
            OwnerId = invocation.OwnerId,
            ProcessName = invocation.ProcessName,
            State = ProcessExecutionState.EState.INITIATED
        };
        ProcessResult result = new()
        {
            ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
            Id = invocation.Id,
            OwnerId = invocation.OwnerId,
            ProcessName = invocation.ProcessName,
            InOuts = new object[0],
            Outputs = new object[] { true },
            TimeStamp = -1
        };
        var operatorInst = DigitalWorkspace.Instance.Operational;
        var res = OnFunction(invocation.ProcessName, operatorInst, state, result);
        await operatorInst.SetResultAsync(result with {InOuts = res.InOuts, Outputs = res.Outputs, TimeStamp = res.TimeStamp });
    }

    public FunctionResult RunFunction(string function, object[] inputs, object[] inOuts)
    {
        if (IsBusy)
        {
            throw new InvalidOperationException("Cannot run function because operator is busy!");
        }
        return OnFunction(function, inputs, inOuts);
    }
}
