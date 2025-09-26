using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;
using VDS.RDF.Nodes;

namespace FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;

public class GetFunctionPropertyDataQuery : ISparqlQuery<FunctionPropertyData>
{
    private readonly ISparqlServer? _sparqlServer;
    private readonly Resource _function;

    // TODO Use config paths!
    public string Query => SparqlHelper.LoadQuery("../FSR.DigitalTwin.App/Sparql/GetFunctionPropertyData.sparql", [_function]);
    public ISparqlResponseParser Parser => new ResponseParser() { Function = _function };
    public ISparqlServer SparqlServer { get => _sparqlServer ?? throw new NullReferenceException(); init => _sparqlServer = value; }

    public GetFunctionPropertyDataQuery(Resource function)
    {
        _function = function;
    }

    private class ResponseParser : ISparqlResponseParser
    {
        public required Resource Function { init; get; }

        public IEnumerable<Triple> FromJson(string jsonResponse)
        {
            var json = JsonDocument.Parse(jsonResponse);
            var bindings = json.RootElement.GetProperty("results").GetProperty("bindings");

            var triples = new List<Triple>();
            foreach (var binding in bindings.EnumerateArray())
            {
                var task = (BaseNode)Function;
                triples.Add(new Triple(task, UriPrefix.RDF | "type", UriPrefix.SOHO | "ProductionTask"));
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasProcedureId", "id");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasProcedureName", "name");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasProcedureDescription", "description");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasDuration", "duration");
                TripleHelper.AddOptionalTriple(binding, triples, task, UriPrefix.SOHO | "hasDurationUncertainty", "durationUncertainty");
            }

            return triples;
        }
    }

    private FunctionPropertyData CreateFunctionData(IEnumerable<Triple> triples)
    {
        string? id = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasProcedureId"))
            .Select(t => t.Object.AsValuedNode().AsString())
            .FirstOrDefault();
        string? name = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasProcedureName"))
            .Select(t => t.Object.AsValuedNode().AsString())
            .FirstOrDefault();
        string? description = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasProcedureDescription"))
            .Select(t => t.Object.AsValuedNode().AsString())
            .FirstOrDefault();
        float duration = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasDuration"))
            .Select(t => t.Object.AsValuedNode().AsFloat())
            .DefaultIfEmpty()
            .Max();
        float durationUncertainty = triples
            .Where(t => t.Predicate as BaseNode == (UriPrefix.SOHO | "hasDurationUncertainty"))
            .Select(t => t.Object.AsValuedNode().AsFloat())
            .DefaultIfEmpty()
            .Max();
        return new FunctionPropertyData()
        {
            Function = _function,
            ProcedureId = id,
            ProcedureName = name,
            ProcedureDescription = description,
            Duration = duration,
            DurationUncertainty = durationUncertainty
        };
    }

    public Result<FunctionPropertyData> Run()
    {
        var response = SparqlServer.Query(this);
        if (response.IsFailure)
        {
            return Result.Failure<FunctionPropertyData>(response.Error);
        }
        return Result.Success(CreateFunctionData(response.Value));
    }

    public async Task<Result<FunctionPropertyData>> RunAsync(CancellationToken cancellationToken = default)
    {
        var response = await SparqlServer.QueryAsync(this, cancellationToken);
        if (response.IsFailure)
        {
            return Result.Failure<FunctionPropertyData>(response.Error);
        }
        return Result.Success(CreateFunctionData(response.Value));
    }
}