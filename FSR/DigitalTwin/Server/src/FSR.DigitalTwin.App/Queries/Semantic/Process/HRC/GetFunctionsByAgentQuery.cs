using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;

public class GetFunctionsByAgentQuery : ISparqlQuery<IEnumerable<Individual>>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _agent;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Queries/Sparql/GetFunctionsByAgent.sparql", [_agent]);
    public ISparqlResponseParser Parser => new ResponseParser() { Agent = _agent };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetFunctionsByAgentQuery(Resource agent)
    {
        _agent = agent;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource Agent { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("task", out JsonElement task_))
                    continue;
                var agent = (BaseNode)Agent;
                var task = RdfNodeFactory.CreateFromJson(task_);
                // triples.Add(new Triple(agent, UriPrefix.RDF | "type", UriPrefix.DUL | "Agent"));
                TripleHelper.AddTriple(binding, triples, agent, UriPrefix.SOHO | "canPerform", "task");
                TripleHelper.AddTriple(binding, triples, task, UriPrefix.RDF | "type", "function");
            }

            return triples;
        }
    }

    public Result<IEnumerable<Individual>> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<Individual>>(response.Error);
        }
        var res = response.Value
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "canPerform"))
            .Select(t => new Individual() { 
                Resource = Resource.FromNode(t.Object),
                Type = [.. response.Value
                    .Where(t0 => t0.Subject as BaseNode == t.Object as BaseNode)
                    .Where(t0 => t0.Predicate as BaseNode == (UriPrefix.RDF | "type"))
                    .Select(t0 => Resource.FromNode(t0.Object))]
            });
        return Result.Success(res);
    }

    public async Task<Result<IEnumerable<Individual>>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<Individual>>(response.Error);
        }
        var res = response.Value
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "canPerform"))
            .Select(t => new Individual() { 
                Resource = Resource.FromNode(t.Object),
                Type = [.. response.Value
                    .Where(t0 => t0.Subject as BaseNode == t.Object as BaseNode)
                    .Where(t0 => t0.Predicate as BaseNode == (UriPrefix.RDF | "type"))
                    .Select(t0 => Resource.FromNode(t0.Object))]
            });
        return Result.Success(res);
    }
}