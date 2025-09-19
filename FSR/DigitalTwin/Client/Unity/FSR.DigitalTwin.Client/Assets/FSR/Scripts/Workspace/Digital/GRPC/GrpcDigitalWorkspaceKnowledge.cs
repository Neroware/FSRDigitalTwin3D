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
            Dictionary<Task, List<Process>> tasks = new();
            Dictionary<Function, SocialOperatorBase> functions = new();

            Dictionary<string, Process> allTasks = new();

            foreach (var goal_ in _client.GetAllGoals(Empty).ResponseStream.ToListAsync().Result)
            {
                var goal = _client.GetProcessDecomposition(goal_);
                var deps = _client.GetProcessDependencies(goal_);
                // TODO Continue with thought here, add tasks based on methods from decomposition, 
                // prevent duplicates of events using the allTasks dictionary! Everything else should be
                // fine! Then we can run processes!
            }

            ProcessSimulation.ProcessSimulationContext context = new(_client)
            {

            };

            return context;
        }
    }
}