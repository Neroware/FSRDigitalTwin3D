using AutoMapper;
using Google.Protobuf;
using VDS.RDF;

namespace FSR.DigitalTwin.App.GRPC.Profiles;

public class BaseProfile : Profile
{
    public BaseProfile()
    {
        // Necessary because protobuf does not have a concept of 'null'
        CreateMap<string, string>().ConvertUsing(s => s ?? string.Empty);

        // Convert System.Byte[] to Google.Protobuf.ByteString
        CreateMap<byte[], ByteString>().ConvertUsing(bytes => ByteString.CopyFrom(bytes));
        CreateMap<ByteString, byte[]>().ConvertUsing(bytes => bytes.ToArray());

        // Convert Uris
        CreateMap<System.Uri, string>().ConvertUsing(uri => uri.ToSafeString());
        CreateMap<string, System.Uri>().ConvertUsing(s => new System.Uri(s));
        CreateMap<Uri, string>()
            .ConvertUsing(src => new System.Uri(src.AbsolutePath).ToSafeString());
        CreateMap<string, Uri>()
            .ConvertUsing((src, dst, ctxt) => ctxt.Mapper.Map<Uri>(new System.Uri(src)));
        CreateMap<System.Uri, Uri>();
        CreateMap<Uri, System.Uri>();
    }
}