using AasxServerStandardBib.Logging;
using AutoMapper;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using Grpc.Core;
using VDS.RDF;

namespace FSR.DigitalTwin.App.GRPC.Services.RPC;

using Uri = System.Uri;

public class HRCProcessSimulationRpcService : HRCProcessSimulationService.HRCProcessSimulationServiceBase
{
    private readonly IAppLogger<HRCProcessSimulationRpcService> _logger;
    private readonly IMapper _mapper;
    private readonly IHRCKnowledgeService _knowledgeBase;
    private readonly IHRCKnowledgeAuthoringService _authoring;
    private readonly IHRCProcessSimulationService _simulation;

    public static readonly Uri uriCobot = UriPrefix.SOHO + "Cobot";
    public static readonly Uri uriRobot = UriPrefix.SOHO + "AutonomousRobot";
    public static readonly Uri uriHuman = UriPrefix.SOHO + "Human";
    public static readonly Uri uriWorkOperator = UriPrefix.SOHO + "WorkOperator";

    public HRCProcessSimulationRpcService(IAppLogger<HRCProcessSimulationRpcService> logger, IMapper mapper, IHRCKnowledgeService knowledgeBase, IHRCKnowledgeAuthoringService authoring, IHRCProcessSimulationService simulation)
    {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        _knowledgeBase = knowledgeBase ?? throw new NullReferenceException(nameof(knowledgeBase));
        _authoring = authoring ?? throw new NullReferenceException(nameof(authoring));
        _simulation = simulation ?? throw new NullReferenceException(nameof(simulation));
    }

    public override Task<HRCProcessSimulationContextDTO> GetSimulationContext(GetSimulationContextRequest request, ServerCallContext context)
    {
        var model = _authoring.CreateModel(request.Horizon);
        HRCProcessSimulationContext ctxt = _simulation.AddModel(new Uri(request.ClientId), model, request.DisplayName);
        _authoring.AttachMetadata(ctxt.Model);
        return Task.FromResult(_mapper.Map<HRCProcessSimulationContextDTO>(ctxt));
    }

    public override Task<Empty> SendSimulationLog(HRCProcessSimulationLogDTO request, ServerCallContext context)
    {
        _simulation.AddLog(new Uri(request.Context.Id), _mapper.Map<HRCProcessSimulationLog>(request));
        return Task.FromResult(new Empty());
    }

    public override async Task GetAllAgents(Empty request, IServerStreamWriter<AgentDTO> responseStream, ServerCallContext context)
    {
        var agents = _knowledgeBase.GetAgents()
            .Select(agent =>
            {
                // TODO Use custom SPARQL query to get agent data more efficiently!
                var agentType = _knowledgeBase.GetResourceType(agent).First();
                Uri foo = UriPrefix.PI + "tmp";
                return new AgentDTO()
                {
                    Id = agent.ToString(),
                    Type = GetAgentType(agentType),
                    Name = "AutonomousAgent"
                };
            });
        foreach (AgentDTO agent in agents)
        {
            await responseStream.WriteAsync(agent);
        }
    }

    public override async Task GetAllGoals(Empty request, IServerStreamWriter<GoalDTO> responseStream, ServerCallContext context)
    {
        var goals = _knowledgeBase.GetGoals()
            .Select(goal => new GoalDTO() { GoalId = goal.ToString() });
        foreach (GoalDTO goal in goals)
        {
            await responseStream.WriteAsync(goal);
        }
    }

    public override Task<FunctionObjectDataDTO> GetFunctionObjectData(HRCTaskDTO request, ServerCallContext context)
    {
        var functionObjectData = _knowledgeBase.GetFunctionObjectProperties(
            request.TaskId.StartsWith('_') ? new Resource() { LocalName = request.TaskId } : new Resource() { Uri = new Uri(request.TaskId) });
        return Task.FromResult(_mapper.Map<FunctionObjectDataDTO>(functionObjectData));
    }

    public override Task<FunctionPropertyDataDTO> GetFunctionPropertyData(HRCTaskDTO request, ServerCallContext context)
    {
        var functionPropertyData = _knowledgeBase.GetFunctionDataProperties(
            request.TaskId.StartsWith('_') ? new Resource() { LocalName = request.TaskId } : new Resource() { Uri = new Uri(request.TaskId) });
        return Task.FromResult(_mapper.Map<FunctionPropertyDataDTO>(functionPropertyData));
    }

    public override Task<GoalDTO> GetProcessDecomposition(GoalDTO request, ServerCallContext context)
    {
        var decompositions = _knowledgeBase.GetDecompositionGraph(
            request.GoalId.StartsWith('_') ? new Resource() { LocalName = request.GoalId }
            : new Resource() { Uri = new Uri(request.GoalId) });

        foreach (var method in decompositions)
        {
            MethodDTO methodDTO = new() { GoalId = request.GoalId };
            foreach (var pair in method)
            {
                TaskDTO disj = new() { TaskId = "", Type = TaskType.Disjuction };
                foreach (ISet<Resource> task in pair.Value)
                {
                    TaskDTO conj = new() { TaskId = "", Type = TaskType.Conjuction };
                    foreach (Resource subTask in task)
                    {
                        conj.SubTasks.Add(new TaskDTO() { TaskId = subTask.ToString(), Type = TaskType.Task });
                    }
                    disj.SubTasks.Add(conj);
                }
                methodDTO.Graph.Add(pair.Key.ToString(), disj);
            }
            request.Methods.Add(methodDTO);
        }
        return Task.FromResult(request);
    }

    // public override Task<TaskDTO> GetProcessDependencies(GoalDTO request, ServerCallContext context)
    // {
    //     var dependencies = _knowledgeBase.GetDependencyGraph(request.GoalId.StartsWith('_') ? new Resource() { LocalName = request.GoalId }
    //         : new Resource() { Uri = new Uri(request.GoalId) });

    //     TaskDTO result = new() { TaskId = request.GoalId, Type = TaskType.Goal };
    //     foreach (var task in dependencies)
    //     {
    //         TaskDTO taskDTO = new() { TaskId = task.Key.Uri.ToSafeString(), Type = TaskType.Task };
    //         foreach (var subTask in task.Value)
    //         {
    //             TaskDTO subTaskDTO = new() { TaskId = subTask.Uri.ToSafeString(), Type = TaskType.Task };
    //             taskDTO.SubTasks.Add(subTaskDTO);
    //         }
    //         result.SubTasks.Add(taskDTO);
    //     }

    //     return Task.FromResult(result);
    // }

    public override Task<InteractionModalityDTO> GetInteractionModality(TaskDTO request, ServerCallContext context)
    {
        var interactionModality = _knowledgeBase.GetInteractionModality(new Resource() { Uri = new Uri(request.TaskId) });
        if (interactionModality == null)
        {
            return Task.FromResult(new InteractionModalityDTO() { Type = InteractionModalityType.None });
        }
        return Task.FromResult(_mapper.Map<InteractionModalityDTO>(interactionModality));
    }

    private static HRCAgentType GetAgentType(Resource type)
    {
        if (type.Uri == uriRobot) return HRCAgentType.Robot;
        if (type.Uri == uriHuman) return HRCAgentType.Human;
        if (type.Uri == uriWorkOperator) return HRCAgentType.WorkerOperator;
        if (type.Uri == uriCobot) return HRCAgentType.Cobot;
        return HRCAgentType.Undefined;
    }
}