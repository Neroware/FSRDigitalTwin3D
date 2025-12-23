using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;

public class GetSkillsQuery : ISparqlQuery<IEnumerable<AgentSkill>>
{
    private readonly ISparqlServer? _sparqlServer;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Queries/Sparql/GetSkillsQuery.sparql", []);
    public ISparqlResponseParser Parser => new ResponseParser();
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    private class ResponseParser : ISparqlResponseParser
    {
        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            Dictionary<INode, INode> capabilities = [];
            Dictionary<INode, List<INode>> methods = [];
            HashSet<INode> skills = [];
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("skill", out JsonElement skill_))
                    continue;
                var skill = RdfNodeFactory.CreateFromJson(skill_);
                skills.Add(skill);

                if (!methods.ContainsKey(skill))
                    methods.Add(skill, []);

                if (!binding.TryGetProperty("capability", out JsonElement capability_))
                    continue;
                var capability = RdfNodeFactory.CreateFromJson(capability_);
                capabilities[skill] = capability;

                if (!binding.TryGetProperty("function", out JsonElement function_))
                    continue;
                var function = RdfNodeFactory.CreateFromJson(function_);
                methods[skill].Add(function);
            }
            foreach (INode skill in skills)
            {
                triples.Add(new Triple(skill, UriPrefix.RDF | "type", UriPrefix.SOBOTS | "Skill"));
                triples.Add(new Triple(skill, UriPrefix.DUL | "describes", capabilities[skill]));
                foreach (INode method in methods[skill])
                {
                    triples.Add(new Triple(skill, UriPrefix.SSN | "implementedBy", method));
                }
            }
            return triples;
        }
    }
    private static IEnumerable<AgentSkill> CreateSkills(IEnumerable<Triple> triples)
    {
        var skills = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.RDF | "type") 
                && t.Object as BaseNode == (UriPrefix.SOBOTS | "Skill"))
            .Select(t => t.Subject.AsResource());
        foreach (Resource skill in skills)
        {
            Resource capability = triples
                .Where(t => t.Predicate as BaseNode == (UriPrefix.DUL | "describes"))
                .Select(t => t.Object.AsResource())
                .First();
            var methods = triples
                .Where(t => t.Predicate as BaseNode == (UriPrefix.SSN | "implementedBy"))
                .Select(t => t.Object.AsResource());
            yield return new AgentSkill(skill) { Capability = capability, Methods = [.. methods] };
        }
    }

    public Result<IEnumerable<AgentSkill>> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<AgentSkill>>(response.Error);
        }
        return Result.Success(CreateSkills(response.Value));
    }

    public async Task<Result<IEnumerable<AgentSkill>>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<AgentSkill>>(response.Error);
        }
        return Result.Success(CreateSkills(response.Value));
    }
}