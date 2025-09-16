using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;

public class GetFunctionObjectDataQuery : ISparqlQuery<FunctionObjectData>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _function;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetFunctionObjectData.sparql", [_function]);
    public ISparqlResponseParser Parser => new ResponseParser();
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetFunctionObjectDataQuery(Resource function)
    {
        _function = function;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                if (!binding.TryGetProperty("task", out JsonElement task_))
                    continue;

                var task = RdfNodeFactory.CreateFromJson(task_);
                triples.Add(new Triple(task, UriPrefix.RDF | "type", UriPrefix.SOHO | "ProductionTask"));
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasTarget", "target");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "requiresStartLocation", "startLoc");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "requiresEndLocation", "endLoc");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "requiresLocation", "loc");
            }

            return triples;
        }
    }

    private FunctionObjectData CreateFunctionData(IEnumerable<Triple> triples)
    {
        var target = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasTarget"))
            .Select(t => Resource.FromNode(t.Object))
            .ToHashSet();
        var startLoc = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "requiresStartLocation"))
            .Select(t => Resource.FromNode(t.Object))
            .ToHashSet();
        var endLoc = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "requiresEndLocation"))
            .Select(t => Resource.FromNode(t.Object))
            .ToHashSet();
        var loc = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "requiresLocation"))
            .Select(t => Resource.FromNode(t.Object))
            .ToHashSet();
        return new FunctionObjectData()
        {
            Function = _function,
            Target = target,
            StartLocation = startLoc,
            EndLocation = endLoc,
            Location = loc
        };
    }

    public Result<FunctionObjectData> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<FunctionObjectData>(response.Error);
        }
        return Result.Success(CreateFunctionData(response.Value));
    }

    public async Task<Result<FunctionObjectData>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<FunctionObjectData>(response.Error);
        }
        return Result.Success(CreateFunctionData(response.Value));
    }
}