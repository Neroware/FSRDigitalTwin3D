using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.App.GRPC;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using Grpc.Core;
using Grpc.Core.Utils;
using UnityEngine;

namespace FSR.DigitalTwin.Client.Features.UnityClient.GRPC
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
                .Where(x => x != null);
            var operators = actors.Select(actor => actor.GetComponent<SocialOperatorBase>());

            var ctxt = _client.CreateSimulationContext(new CreateSimulationContextRequest()
            {
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                DisplayName = "My Simulation",
                Horizon = 1000000
            });
            var model = ctxt.Model;

            Dictionary<Goal, IList<Method>> goals = new();
            Dictionary<Method, IDictionary<Task, IList<ISet<Task>>>> methods = new();
            Dictionary<string, Task> tasks = new();
            Dictionary<Task, List<HashSet<string>>> subTasks = new();
            int methodCounter = 0;

            foreach (var goal_ in _client.GetAllGoals(Empty).ResponseStream.ToListAsync().Result)
            {
                var goal = _client.GetProcessDecomposition(goal_);
                Goal g = new() { GoalId = goal.GoalId, GoalName = goal.GoalId };
                if (!goals.ContainsKey(g))
                {
                    goals.Add(g, new List<Method>());
                }
                foreach (var method in goal.Methods)
                {
                    Method m = new() { Goal = g, MethodId = methodCounter++ };
                    if (!methods.ContainsKey(m))
                    {
                        methods.Add(m, new Dictionary<Task, IList<ISet<Task>>>());
                    }
                    foreach (var (taskId, task) in method.Graph)
                    {
                        if (!task.SubTasks.Any())
                        {
                            Task t;
                            if (model.Tasks.Select(hrcTask => hrcTask.Id).Contains(taskId))
                            {
                                // TODO Retreive addtional function data...
                                t = new Function() { TaskId = taskId, Name = taskId, Operator = null, Actor = null };
                            }
                            else
                            {
                                t = new Task(ETaskType.Basic) { TaskId = taskId, Name = taskId };
                            }
                            if (!tasks.ContainsKey(taskId))
                            {
                                tasks.Add(taskId, t);
                                subTasks.Add(t, new());
                            }
                        }
                        else
                        {
                            if (task.Type != TaskType.Disjuction)
                            {
                                throw new System.Exception("wrong format in decomposition graph");
                            }
                            Task t = new(ETaskType.Complex) { TaskId = taskId, Name = taskId };
                            if (!tasks.ContainsKey(taskId))
                            {
                                tasks.Add(taskId, t);
                                subTasks.Add(t, new());
                            }
                            foreach (TaskDTO subTask in task.SubTasks)
                            {
                                if (subTask.Type != TaskType.Conjuction)
                                {
                                    throw new System.Exception("wrong format in decomposition graph");
                                }
                                HashSet<string> ts = new(subTask.SubTasks.Select(x => x.TaskId));
                                subTasks[t].Add(ts);
                            }
                        }
                    }
                    foreach (var taskId in method.Graph.Keys)
                    {
                        Task t = tasks[taskId];
                        methods[m].Add(t, new List<ISet<Task>>());
                        var taskDecomp = subTasks[t].Select(ts => ts.Select(x => tasks[x]).ToHashSet()).Cast<ISet<Task>>();
                        foreach (var decomp in taskDecomp)
                        {
                            methods[m][t].Add(decomp);
                        }
                    }
                }
            }

            return new ProcessSimulation.ProcessSimulationContext()
            {
                Actors = actors.ToList(),
                Goals = goals,
                Functions = tasks.Values.Where(t => t.ProcessType == EProcessType.Function).Cast<Function>().ToList(),
                Methods = methods,
                Simulation = null
            };
        }
    }
}