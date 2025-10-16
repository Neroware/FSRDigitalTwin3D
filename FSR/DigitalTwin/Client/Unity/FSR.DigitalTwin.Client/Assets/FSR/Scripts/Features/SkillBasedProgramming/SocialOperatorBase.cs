using System;
using UniRx;
using Unity.VisualScripting;
using UnityEngine;
using FSR.DigitalTwin.Client.Features.UnityClient;
using FSR.DigitalTwin.Client.Features.UnityClient.GRPC;
using System.Threading.Tasks;
using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.SkillBasedProgramming.Interfaces;

namespace FSR.DigitalTwin.Client.Features.SkillBasedProgramming
{
    public abstract class SocialOperatorBase : DigitalTwinComponentBase, ISocialOperator
    {
        [SerializeField] private string operatorId = "";

        public abstract bool IsBusy { get; }
        public abstract string RunningOperation { get; }

        protected abstract Task<HRCProcessResult<HRCFunction>> OnFunction(string function, object[] inputs, object[] inOuts);

        public Uri OperatorId => operatorId.Length == 0 ? Id : new(operatorId);

        protected override void OnConnect()
        {
            DigitalWorkspace.Instance.Operational.ProcessInvoked
                .Where(i => i.OwnerId == Id.ToSafeString())
                .Subscribe(RunFunction).AddTo(this);
        }

        public async Task RunFunctionAsync(ProcessInvocation invocation)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
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
            ProcessExecutionState state = new()
            {
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                Id = invocation.Id,
                OwnerId = invocation.OwnerId,
                ProcessName = invocation.ProcessName,
                State = ProcessExecutionState.EState.INITIATED
            };
            var res = await OnFunction(invocation.ProcessName, invocation.Inputs, invocation.InOuts);
            if (res.Failed)
            {
                await DigitalWorkspace.Instance.Operational
                    .SetExecutionProcessStateAsync(state with { State = ProcessExecutionState.EState.FAILED });
                return;
            }
            await DigitalWorkspace.Instance.Operational
                .SetExecutionProcessStateAsync(state with { State = ProcessExecutionState.EState.COMPLETED });
            await DigitalWorkspace.Instance.Operational
                .SetResultAsync(result with { InOuts = res.InOuts, Outputs = res.Outputs, TimeStamp = (long)res.TimeStamp.TimeOfDay.TotalSeconds });
        }
        public async void RunFunction(ProcessInvocation invocation)
        {
            await RunFunctionAsync(invocation);
        }
        public HRCProcessResult<HRCFunction> RunFunction(string function, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
            return OnFunction(function, inputs, inOuts).Result;
        }
        public async Task<HRCProcessResult<HRCFunction>> RunFunctionAsync(string function, object[] inputs, object[] inOuts)
        {
            if (IsBusy)
            {
                throw new InvalidOperationException("Cannot run function because operator is busy!");
            }
            return await OnFunction(function, inputs, inOuts);
        }
    }

}