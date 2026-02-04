using System.Text.Json;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Common.Utils.Semantic;

public static class TripleHelper
{
    public static void AddTripleOrDefault(JsonElement binding, List<Triple> triples, INode subject, INode predicate, string propertyName, BaseNode default_)
    {
        if (binding.TryGetProperty(propertyName, out JsonElement element))
        {
            var node = RdfNodeFactory.CreateFromJson(element);
            triples.Add(new Triple(subject, predicate, node));
        }
        else
        {
            triples.Add(new Triple(subject, predicate, default_));
        }
    }

    public static void AddOptionalTriple(JsonElement binding, List<Triple> triples, INode subject, INode predicate, string propertyName, INode? objectType = null)
    {
        if (binding.TryGetProperty(propertyName, out JsonElement element))
        {
            var node = RdfNodeFactory.CreateFromJson(element);
            if (objectType != null)
                triples.Add(new Triple(node, UriPrefix.RDF | "type", objectType));
            triples.Add(new Triple(subject, predicate, node));
        }
    }
    public static void AddTriple(JsonElement binding, List<Triple> triples, INode subject, INode predicate, string propertyName, INode? objectType = null)
    {
        var element = binding.GetProperty(propertyName);
        var node = RdfNodeFactory.CreateFromJson(element);
        if (objectType != null)
            triples.Add(new Triple(node, UriPrefix.RDF | "type", objectType));
        triples.Add(new Triple(subject, predicate, node));
    }
    public static void AddTriple(JsonElement binding, List<Triple> triples, string subjectName, string propertyName, string objectName)
    {
        var s = binding.GetProperty(subjectName);
        var subj = RdfNodeFactory.CreateFromJson(s);
        var p = binding.GetProperty(propertyName);
        var prop = RdfNodeFactory.CreateFromJson(p);
        var o = binding.GetProperty(objectName);
        var obj = RdfNodeFactory.CreateFromJson(o);
        triples.Add(new Triple(subj, prop, obj));
    }
    public static void AddTriple(JsonElement binding, List<Triple> triples, string subjectName, INode property, string objectName)
    {
        var s = binding.GetProperty(subjectName);
        var subj = RdfNodeFactory.CreateFromJson(s);
        var o = binding.GetProperty(objectName);
        var obj = RdfNodeFactory.CreateFromJson(o);
        triples.Add(new Triple(subj, property, obj));
    }
    public static void AddTriple(List<Triple> triples, INode s, INode p, INode o) => triples.Add(new Triple(s, p, o));
    public static void AddType(List<Triple> triples, INode s, INode type) => triples.Add(new Triple(s, UriPrefix.RDF | "type", type));
}