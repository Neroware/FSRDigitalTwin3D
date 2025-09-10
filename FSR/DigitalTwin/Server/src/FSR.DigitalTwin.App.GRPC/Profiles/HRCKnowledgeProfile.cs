using AutoMapper;
using VDS.RDF;

namespace FSR.DigitalTwin.App.GRPC.Profiles;

public class HRCKnowledgeProfile : Profile
{
    public HRCKnowledgeProfile()
    {
        CreateMap<Uri, string>().ConvertUsing(uri => uri.ToSafeString());
        CreateMap<string, Uri>().ConvertUsing(s => new Uri(s));
        CreateDomainMappings();
        CreateModelMappings();
    }

    private void CreateDomainMappings()
    {

    }

    private void CreateModelMappings()
    {
        
    }
}