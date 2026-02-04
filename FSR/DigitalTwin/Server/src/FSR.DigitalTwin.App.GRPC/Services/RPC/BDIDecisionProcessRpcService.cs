using AasxServerStandardBib.Logging;
using AutoMapper;
using FSR.DigitalTwin.App.GRPC.Process.BDI;
using FSR.DigitalTwin.App.GRPC.Process.BDI.Services.BDIDecisionProcessService;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.BDI;
using FSR.DigitalTwin.Domain.Model.Process.BDI;
using Grpc.Core;

namespace FSR.DigitalTwin.App.GRPC.Services.RPC;

public class BDIDecisionProcessRpcService : BDIDecisionProcessService.BDIDecisionProcessServiceBase
{
    private readonly IAppLogger<BDIDecisionProcessRpcService> _logger;
    private readonly IMapper _mapper;
    private readonly IBDIDecisionProcessService _decisionProcess;

    public BDIDecisionProcessRpcService(IAppLogger<BDIDecisionProcessRpcService> logger, IMapper mapper, IBDIDecisionProcessService decisionProcess) {
        _logger = logger ?? throw new NullReferenceException();
        _mapper = mapper ?? throw new NullReferenceException();
        _decisionProcess = decisionProcess ?? throw new NullReferenceException();
    }

    public override Task<ActionDTO> RunDecisionProcess(RunDecisionProcessRequest request, ServerCallContext context)
    {
        BDIActionIntent action = _decisionProcess.GetDecisionProcessActionIntent(
            new Domain.Model.Resource() { Uri = new System.Uri(request.Id) });
        return Task.FromResult(_mapper.Map<ActionDTO>(action));
    }
}