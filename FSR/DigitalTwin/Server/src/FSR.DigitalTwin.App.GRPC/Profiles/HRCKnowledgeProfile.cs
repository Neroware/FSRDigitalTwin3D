using AutoMapper;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.GRPC.Process.BDI;
using FSR.DigitalTwin.App.GRPC.Process.HRC;
using FSR.DigitalTwin.App.GRPC.Process.HRC.Services.HRCProcessSimulationService;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process;
using FSR.DigitalTwin.Domain.Model.Process.BDI;
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
            .ForMember(dest => dest.Target, opt => opt.MapFrom(src => src.Target == null ? "" : src.Target.ToString()))
            .ForMember(dest => dest.TaskId, opt => opt.MapFrom(src => src.Resource.ToString()));
        CreateMap<AgentSkill, HRCSkillDTO>()
            .ForMember(dest => dest.SkillId, opt => opt.MapFrom(src => src.Resource.ToString()))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => UriPrefix.SOBOTS + "Skill"));
        CreateMap<HRCModel, HRCModelDTO>();
        CreateMap<FunctionObjectData, FunctionObjectDataDTO>()
            .ForMember(dest => dest.FunctionId, opt => opt.MapFrom(src => src.Function.Uri.ToSafeString()));
        CreateMap<FunctionPropertyData, FunctionPropertyDataDTO>()
            .ForMember(dest => dest.FunctionId, opt => opt.MapFrom(src => src.Function.Uri.ToSafeString()));
        CreateMap<HRCProcessSimulationContext, HRCProcessSimulationContextDTO>();
        CreateMap<InteractionModality, InteractionModalityDTO>()
            .ForMember(dest => dest.Type, opt => opt.MapFrom(src => ConvertInteractionModalityType(src.Type)))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.ToString()));
        CreateMap<BDIActionIntent, ActionDTO>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Resource.Uri.ToSafeString()));
    }

    private void CreateModelMappings()
    {
        CreateMap<HRCProcessSimulationLogDTO, HRCProcessSimulationLog>();
    }

    private static InteractionModalityType ConvertInteractionModalityType(Resource resource)
    {
        if (resource.Uri != UriPrefix.SOHO + "")
        {
            return InteractionModalityType.None;
        }
        return resource.Uri.Fragment switch
        {
            "#Simultaneous" => InteractionModalityType.Simultaneous,
            "#Sequential" => InteractionModalityType.Sequential,
            "#Supportive" => InteractionModalityType.Supportive,
            "#Independent" => InteractionModalityType.Independent,
            _ => InteractionModalityType.None
        };
    }
}