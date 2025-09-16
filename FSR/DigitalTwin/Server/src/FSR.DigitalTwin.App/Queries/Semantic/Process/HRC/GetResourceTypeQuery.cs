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

public class GetResourceTypeQuery : ISparqlQuery<IEnumerable<Resource>>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _resource;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetResourceType.sparql", [_resource]);
    public ISparqlResponseParser Parser => new ResponseParser() { Resource = _resource };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetResourceTypeQuery(Resource resource)
    {
        _resource = resource;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource Resource { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("type", out JsonElement type_))
                    continue;
                var type = RdfNodeFactory.CreateFromJson(type_);
                triples.Add(new Triple((BaseNode)Resource, UriPrefix.RDF | "type", type));
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
        return Result.Success(response.Value.Select(t => Resource.FromNode(t.Object)));
    }

    public Result<IEnumerable<Resource>> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<IEnumerable<Resource>>(response.Error);
        }
        return Result.Success(response.Value.Select(t => Resource.FromNode(t.Object)));
    }
}