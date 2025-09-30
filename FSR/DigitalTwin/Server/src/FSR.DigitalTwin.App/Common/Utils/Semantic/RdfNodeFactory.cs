using System.Text.Json;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Common.Utils.Semantic;

public static class RdfNodeFactory
{

    public static INode CreateFromJson(JsonElement node)
    {
        string? type = node.TryGetProperty("type", out JsonElement typeElem) ? typeElem.GetString() : null;
        string? value = node.TryGetProperty("value", out JsonElement valueElem) ? valueElem.GetString() : null;
        string? datatype = node.TryGetProperty("datatype", out JsonElement datatypeElem) ? datatypeElem.GetString() : null;

        return type switch
        {
            "uri" => new UriNode(new Uri(value ?? throw new FormatException("URI not provided"))),
            "bnode" => new BlankNode(value ?? throw new FormatException("Local id not provided")),
            "literal" => datatype != null ? new LiteralNode(value ?? "", new Uri(datatype)) : new LiteralNode(value ?? ""),
            _ => throw new FormatException("Unable to create node from JSON")
        };
    }

    public static INode CreateFromJson(JsonElement binding, string property) => CreateFromJson(binding.GetProperty(property));

}