using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.BDI;
using FSR.DigitalTwin.Domain.SharedKernel;
using Microsoft.IdentityModel.Tokens;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Queries.Semantic.Process.BDI;

public class GetActionIntent : ISparqlQuery<BDIActionIntent>
{
    private readonly Resource _decisionProcess;
    private readonly ISparqlServer? _sparqlServer;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Queries/Sparql/GetActionIntent.sparql", [_decisionProcess]);
    public ISparqlResponseParser Parser => new ResponseParser();
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    private class ResponseParser : ISparqlResponseParser
    {
        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");
            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                TripleHelper.AddTriple(binding, triples, "action", UriPrefix.DUL | "isDescribedBy", "task");
            }
            return triples;
        }
    }

    public GetActionIntent(Resource decisionProcess)
    {
        _decisionProcess = decisionProcess;
    }

    public Result<BDIActionIntent> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<BDIActionIntent>(response.Error);
        }
        return Result.Success(new BDIActionIntent()
        {
            Resource = response.Value.IsNullOrEmpty() ? 
                new Resource() { LocalName = "<<unknown action>>" } :
                Resource.FromNode(response.Value.First().Subject),
            Decision = _decisionProcess,
            Tasks = [.. response.Value.Select(t => Resource.FromNode(t.Object))]
        });
    }
    
    public async Task<Result<BDIActionIntent>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<BDIActionIntent>(response.Error);
        }
        return Result.Success(new BDIActionIntent()
        {
            Resource = response.Value.IsNullOrEmpty() ? 
                new Resource() { LocalName = "<<unknown action>>" } :
                Resource.FromNode(response.Value.First().Subject),
            Decision = _decisionProcess,
            Tasks = [.. response.Value.Select(t => Resource.FromNode(t.Object))]
        });
    }
}

