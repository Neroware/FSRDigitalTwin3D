using AutoMapper;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using VDS.RDF;

namespace FSR.DigitalTwin.App.GRPC.Profiles;

public class HRCKnowledgeProfile : Profile
{
    public HRCKnowledgeProfile()
    {
        CreateDomainMappings();
        CreateModelMappings();
    }

    private void CreateDomainMappings()
    {
        CreateMap<HRCTask, HRCTaskDTO>()
            .ForMember(dest => dest.Agent, opt => opt.MapFrom(src => (AgentType)src.Agent))
            .ForMember(dest => dest.MinDuration, opt => opt.MapFrom(src => src.Duration.Item1))
            .ForMember(dest => dest.MaxDuration, opt => opt.MapFrom(src => src.Duration.Item2))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description ?? ""))
            .ForMember(dest => dest.Goal, opt => opt.MapFrom(src => src.Goal ?? ""))
            .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target == null ? "" : src.Target.ToString()));
        CreateMap<HRCModel, HRCModelDTO>();
        CreateMap<FunctionObjectData, FunctionObjectDataDTO>()
            .ForMember(dest => dest.FunctionId, opt => opt.MapFrom(src => src.Function.Uri.ToSafeString()));
        CreateMap<FunctionPropertyData, FunctionPropertyDataDTO>()
            .ForMember(dest => dest.FunctionId, opt => opt.MapFrom(src => src.Function.Uri.ToSafeString()));
        CreateMap<HRCProcessSimulationContext, HRCProcessSimulationContextDTO>();
    }

    private void CreateModelMappings()
    {
        CreateMap<HRCProcessSimulationLogDTO, HRCProcessSimulationLog>();
    }
}