
using System.Text;
using VDS.RDF;

namespace FSR.DigitalTwin.Domain.Model;

public class Resource : SharedKernel.Resource.Resource
{
    private readonly string _name = "";
    private readonly byte[] _bytes = [];

    public Resource(string name = "", byte[]? bytes = null)
    {
        _name = name;
        _bytes = bytes ?? [];
    }

    public override Uri? Uri { get; init; }
    public override string? LocalName { get; init; }
    public override string? Name => _name;
    public override byte[] GetBytes() => _bytes;
    public override int GetContentLength() => _bytes.Length;

    public byte[] Data => GetBytes();
    public int Length => GetContentLength();

    public static implicit operator Resource(BaseNode node) => node.NodeType switch
    {
        NodeType.Uri => new Resource() { Uri = ((UriNode)node).Uri, LocalName = null },
        NodeType.Blank => new Resource() { Uri = null, LocalName = ((BlankNode)node).InternalID },
        NodeType.Literal => new Resource(bytes: Encoding.ASCII.GetBytes(((LiteralNode)node).Value)),
        _ => throw new ArgumentException("should not happen")
    };

    public override string ToString() => Uri != null ? Uri.ToSafeString() : LocalName ?? "";
}