using System;
using System.Collections.Generic;
using System.Linq;
using FSR.DigitalTwin.App.GRPC;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Client.Common.Utils.Semantic;
using FSR.DigitalTwin.Client.Features.DES;
using FSR.DigitalTwin.Client.Features.DES.Interfaces;
using FSR.DigitalTwin.Client.Features.UnityClient.Interfaces;
using Grpc.Core;
using Grpc.Core.Utils;

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
                .Select(actor => UnityEngine.Object.FindObjectsOfType<DigitalTwinActorBase>()
                    .FirstOrDefault(sceneActor => sceneActor.TryGetComponent(out SocialOperatorBase op) && op.OperatorId == new System.Uri(actor.Id)))
                .Where(x => x != null);
            var operators = actors.Select(actor => actor.GetComponent<SocialOperatorBase>());

            var ctxt = _client.GetSimulationContext(new GetSimulationContextRequest()
            {
                ClientId = GrpcDigitalWorkspaceConnection.UNITY_CLIENT_ID,
                DisplayName = "My Simulation",
                Horizon = 1000000
            });
            var model = ctxt.Model;

            Dictionary<HRCGoal, IList<HRCMethod>> goals = new();
            Dictionary<HRCMethod, IDictionary<HRCTask, IList<ISet<HRCTask>>>> methods = new();
            Dictionary<string, HRCTask> tasks = new();
            Dictionary<HRCTask, List<HashSet<string>>> subTasks = new();
            int methodCounter = 0;

            foreach (var goal_ in _client.GetAllGoals(Empty).ResponseStream.ToListAsync().Result)
            {
                var goal = _client.GetProcessDecomposition(goal_);
                HRCGoal g = new() { GoalId = goal.GoalId, GoalName = goal.GoalId };
                if (!goals.ContainsKey(g))
                {
                    goals.Add(g, new List<HRCMethod>());
                }
                foreach (var method in goal.Methods)
                {
                    HRCMethod m = new() { Goal = g, MethodId = methodCounter++ };
                    if (!methods.ContainsKey(m))
                    {
                        methods.Add(m, new Dictionary<HRCTask, IList<ISet<HRCTask>>>());
                    }
                    foreach (var (taskId, task) in method.Graph)
                    {
                        if (!task.SubTasks.Any())
                        {
                            HRCTask t;
                            HRCTaskDTO taskDTO = model.Tasks.Where(hrcTask => hrcTask.TaskId == taskId).FirstOrDefault();
                            if (taskDTO != null)
                            {
                                HRCFunctionDescription description = new()
                                {
                                    TaskType = EHRCTaskType.Basic,
                                    Name = taskDTO.Name,
                                    Duration = TimeSpan.FromSeconds(taskDTO.AverageDuration),
                                    MaxDuration = TimeSpan.FromSeconds(taskDTO.MaxDuration),
                                    MinDuration = TimeSpan.FromSeconds(taskDTO.MinDuration),
                                    DurationUncertainty = TimeSpan.FromSeconds(taskDTO.DurationUncertainty),
                                    AgentType = (EHRCAgentType)taskDTO.Agent,
                                    SuccessRate = taskDTO.SuccessRate,
                                    Description = taskDTO.Description,
                                    Id = taskDTO.Id,
                                    Target = taskDTO.Target,
                                    StartLocation = taskDTO.StartLocation,
                                    EndLocation = taskDTO.EndLocation,
                                    Location = taskDTO.Location
                                };
                                t = new HRCFunction()
                                {
                                    TaskId = taskId,
                                    TaskDescription = description
                                };
                            }
                            else
                            {
                                t = new HRCTask() { TaskId = taskId };
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
                                throw new Exception("wrong format in decomposition graph");
                            }
                            HRCTask t = new() { TaskId = taskId };
                            if (!tasks.ContainsKey(taskId))
                            {
                                tasks.Add(taskId, t);
                                subTasks.Add(t, new());
                            }
                            foreach (TaskDTO subTask in task.SubTasks)
                            {
                                if (subTask.Type != TaskType.Conjuction)
                                {
                                    throw new Exception("wrong format in decomposition graph");
                                }
                                HashSet<string> ts = new(subTask.SubTasks.Select(x => x.TaskId));
                                subTasks[t].Add(ts);
                            }
                        }
                    }
                    foreach (var taskId in method.Graph.Keys)
                    {
                        HRCTask t = tasks[taskId];
                        methods[m].Add(t, new List<ISet<HRCTask>>());
                        var taskDecomp = subTasks[t].Select(ts => ts.Select(x => tasks[x]).ToHashSet()).Cast<ISet<HRCTask>>();
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
                Functions = tasks.Values.Where(t => t.ProcessType == EHRCProcessType.Function).Cast<HRCFunction>().ToList(),
                Methods = methods,
                Simulation = null
            };
        }
    }
}