using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.App.GRPC;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Client.Unity.Workspace.Digital.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Interfaces;
using FSR.DigitalTwin.Client.Unity.Workspace.Virtual.Process;
using Grpc.Core;
using Grpc.Core.Utils;
using Unity.VisualScripting;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Unity.Workspace.Digital.GRPC
{
    public class GrpcDigitalWorkspaceKnowledge : IDigitalWorkspaceKnowledge
    {
        private static readonly Empty Empty = new();
        public Channel RpcChannel => _rpcChannel ?? throw new RpcException(Status.DefaultCancelled, "No connection established!");
        private Channel _rpcChannel = null;
        private HRCProcessSimulationService.HRCProcessSimulationServiceClient _client;

        public GrpcDigitalWorkspaceKnowledge(Channel rpcChannel)
        {
            _rpcChannel = rpcChannel;
            _client = new(rpcChannel);
        }

        public IProcessSimulationContext GetContext()
        {
            var actors = _client.GetAllAgents(Empty).ResponseStream.ToListAsync().Result
                .Select(actor => Object.FindObjectsOfType<DigitalTwinActorBase>()
                    .FirstOrDefault(sceneActor => sceneActor.TryGetComponent(out SocialOperatorBase op) && op.OperatorId == new System.Uri(actor.Id)))
                .NotNull();
            var operators = actors.Select(actor => actor.GetComponent<SocialOperatorBase>());

            Dictionary<Goal, List<Method>> goals = new();
            Dictionary<Method, List<Process>> methods = new();
            Dictionary<Task, List<Task>> tasks = new();
            Dictionary<Function, SocialOperatorBase> functions = new();

            foreach (var goal in _client.GetAllGoals(Empty).ResponseStream.ToListAsync().Result)
            {
                var goalDecompositions = _client.DecomposeProcess(goal).ResponseStream.ToListAsync().Result;
                System.Uri goalId = new(goal.GoalId);
                goals.Add(new Goal() { ProcessId = goalId, GoalName = goalId.Fragment }, new List<Method>());
                foreach (var method in goalDecompositions)
                {
                    foreach (string taskId in method.Graph.Keys)
                    {
                        
                    }
                }
            }

            // Dictionary<Goal, Method>
            // var goals = _client.GetAllGoals(Empty).ResponseStream.ToListAsync().Result
            //     .Select(goal =>
            //     {
            //         

            //     });

            ProcessSimulation.ProcessSimulationContext context = new(_client)
            {

            };

            return context;
        }
    }
}