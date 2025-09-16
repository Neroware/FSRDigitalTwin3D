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

public class GetPropertyQuery : ISparqlQuery<IEnumerable<Resource>>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _property;
    private readonly Resource _individual;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetProperty.sparql",
        [_property, _individual]);
    public ISparqlResponseParser Parser => new ResponseParser() { Property = _property, Individual = _individual };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetPropertyQuery(Resource property, Resource individual)
    {
        _property = property;
        _individual = individual;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource Property { init; get; }
        public required Resource Individual { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("value", out JsonElement value_))
                    continue;
                var value = RdfNodeFactory.CreateFromJson(value_);
                triples.Add(new Triple((BaseNode)Individual, (BaseNode)Property, value));
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