using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using FSR.DigitalTwin.Client.Features.UnityClient;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC;

namespace FSR.DigitalTwin.Client.Features.DES
{
    public abstract class SocialOperatorBase : DigitalTwinComponentBase, ISocialOperator
    {
        [SerializeField] private string operatorId = "";

        public abstract bool IsBusy { get; }
        public abstract string RunningOperation { get; }

        protected abstract FunctionResult OnFunction(string function, IDigitalWorkspaceOperational operatorInst, ProcessExecutionState state, UnityClient.ProcessResult result);
        protected abstract FunctionResult OnFunction(string function, object[] inputs, object[] inOuts);

        public Uri OperatorId => operatorId.Length == 0 ? Id : new(operatorId);

        protected override void OnConnect()
        {
            DigitalWorkspace.Instance.Operational.ProcessInvoked
                .Where(i => i.OwnerId == Id.ToSafeString())
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
            UnityClient.ProcessResult result = new()
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
            await operatorInst.SetResultAsync(result with { InOuts = res.InOuts, Outputs = res.Outputs, TimeStamp = res.TimeStamp });
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

}