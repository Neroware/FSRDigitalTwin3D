
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
    private Resource(UriNode node, string name = "", byte[]? bytes = null)
    {
        Uri = node.Uri;
        LocalName = null;
        _name = name;
        _bytes = bytes ?? [];
    }
    private Resource(BlankNode node, string name = "", byte[]? bytes = null)
    {
        Uri = null;
        LocalName = node.InternalID;
        _name = name;
        _bytes = bytes ?? [];
    }
    private Resource(LiteralNode node)
    {
        _bytes = Encoding.ASCII.GetBytes(node.Value);
    }

    public override Uri? Uri { get; init; }
    public override string? LocalName { get; init; }
    public override string? Name => _name;
    public override byte[] GetBytes() => _bytes;
    public override int GetContentLength() => _bytes.Length;

    public byte[] Data => GetBytes();
    public int Length => GetContentLength();

    public static explicit operator Resource(BaseNode node) => node.NodeType switch
    {
        NodeType.Uri => new Resource((UriNode)node),
        NodeType.Blank => new Resource((BlankNode)node),
        NodeType.Literal => new Resource((LiteralNode)node),
        _ => throw new ArgumentException("should not happen")
    };
    public static explicit operator Resource(UriNode node) => new(node);
    public static explicit operator Resource(BlankNode node) => new(node);
    public static explicit operator Resource(LiteralNode node) => new(node);

    public static explicit operator BaseNode(Resource resource)
    {
        if (resource.Uri != null)
        {
            return new UriNode(resource.Uri);
        }
        if (resource.LocalName != null)
        {
            return new BlankNode(resource.LocalName);
        }
        throw new InvalidCastException("missing identifier");
    }
    public static implicit operator Resource(Uri uri) => new() { Uri = uri };

    public static Resource FromNode(INode node) => node is BaseNode n ? ((Resource)n) : throw new ArgumentException("should not happen");

    public override string ToString() => Uri?.ToSafeString() ?? $"_:{LocalName ?? "_"}";
    public override StreamReader GetStreamReader() => new(new MemoryStream(_bytes));
}