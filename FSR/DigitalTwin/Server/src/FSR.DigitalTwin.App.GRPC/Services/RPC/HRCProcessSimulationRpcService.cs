using AasxServerStandardBib.Logging;
using AutoMapper;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using Grpc.Core;

namespace FSR.DigitalTwin.App.GRPC.Services.RPC;

public class HRCProcessSimulationRpcService : HRCProcessSimulationService.HRCProcessSimulationServiceBase
{
    private readonly IAppLogger<HRCProcessSimulationRpcService> _logger;
    private readonly IMapper _mapper;
    private readonly IHRCKnowledgeService _knowledgeBase;

    public HRCProcessSimulationRpcService(IAppLogger<HRCProcessSimulationRpcService> logger, IMapper mapper, IHRCKnowledgeService knowledgeBase)
    {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _mapper = mapper ?? throw new NullReferenceException(nameof(mapper));
        _knowledgeBase = knowledgeBase ?? throw new NullReferenceException(nameof(knowledgeBase));
    }

    public override Task<HRCModelDTO> CreateHRCModel(Empty request, ServerCallContext context)
    {
        return base.CreateHRCModel(request, context);
    }
}