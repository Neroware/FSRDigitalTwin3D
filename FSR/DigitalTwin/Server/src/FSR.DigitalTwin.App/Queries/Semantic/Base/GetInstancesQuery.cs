namespace FSR.DigitalTwin.App.Queries.Semantic.Base;

using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

public class GetInstancesQuery : ISparqlQuery<IEnumerable<Resource>>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _type;

    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetInstances.sparql", [_type]);
    public ISparqlResponseParser Parser => new ResponseParser() { Type = _type };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetInstancesQuery(Resource type)
    {
        _type = type;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource Type { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("instance", out JsonElement instance_))
                    continue;
                var instance = RdfNodeFactory.CreateFromJson(instance_);
                triples.Add(new Triple(instance, UriPrefix.RDF | "type", (BaseNode)Type));
            }
            return triples;
        }
    }

    public async Task<Result<IEnumerable<Resource>>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<Resource>>(response.Error);
        }
        return Result.Success(response.Value.Select(t => Resource.FromNode(t.Subject)).Distinct());
    }

    public Result<IEnumerable<Resource>> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<Resource>>(response.Error);
        }
        return Result.Success(response.Value.Select(t => Resource.FromNode(t.Subject)).Distinct());
    }
}