using AasxServerStandardBib.Logging;
using AutoMapper;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using Grpc.Core;

namespace FSR.DigitalTwin.App.GRPC.Services.RPC;

public class HRCProcessSimulationRpcService : HRCProcessSimulationService.HRCProcessSimulationServiceBase
{
    private readonly IAppLogger<HRCProcessSimulationRpcService> _logger;
    private readonly IMapper _mapper;
    private readonly IHRCKnowledgeService _knowledgeBase;
    private readonly IHRCKnowledgeAuthoringService _authoring;
    private readonly IHRCProcessSimulationService _simulation;

    public HRCProcessSimulationRpcService(IAppLogger<HRCProcessSimulationRpcService> logger, IMapper mapper, IHRCKnowledgeService knowledgeBase, IHRCKnowledgeAuthoringService authoring, IHRCProcessSimulationService simulation)
    {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        _knowledgeBase = knowledgeBase ?? throw new NullReferenceException(nameof(knowledgeBase));
        _authoring = authoring ?? throw new NullReferenceException(nameof(authoring));
        _simulation = simulation ?? throw new NullReferenceException(nameof(simulation));
    }

    public override Task<HRCProcessSimulationContextDTO> CreateSimulationContext(CreateSimulationContextRequest request, ServerCallContext context)
    {
        var model = _authoring.CreateModel(request.Horizon);
        var ctxt = _simulation.AddModel(new Uri(request.ClientId), model, request.DisplayName);
        return Task.FromResult(_mapper.Map<HRCProcessSimulationContextDTO>(ctxt));
    }

    public override Task<Empty> SendSimulationLog(HRCProcessSimulationLogDTO request, ServerCallContext context)
    {
        _simulation.AddLog(new Uri(request.Context.Id), _mapper.Map<HRCProcessSimulationLog>(request));
        return Task.FromResult(new Empty());
    }

    public override Task GetAllAgents(Empty request, IServerStreamWriter<AgentDTO> responseStream, ServerCallContext context)
    {
        return base.GetAllAgents(request, responseStream, context);
    }
}