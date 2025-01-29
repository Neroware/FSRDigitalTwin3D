using System.Text.Json;
using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Queries.Semantic.Example;

public class GetAllTripletsQuery : ISparqlQuery<IEnumerable<Triple>>
{
    public ISparqlServer SparqlServer { get; init; }
    public string Query => "SELECT * WHERE { ?s ?p ?o . } ORDER BY ?p ?o";
    public ISparqlResponseParser Parser => new ResponseParser();

    public GetAllTripletsQuery(ISparqlServer sparqlServer) {
        SparqlServer = sparqlServer;
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
                var subject = binding.GetProperty("s").GetProperty("value").GetString() ?? throw new FormatException();
                var predicate = binding.GetProperty("p").GetProperty("value").GetString() ?? throw new FormatException();
                var obj = binding.GetProperty("o").GetProperty("value").GetString() ?? throw new FormatException();

                triples.Add(new Triple(new UriNode(new Uri(subject)), 
                            new UriNode(new Uri(predicate)), 
                            new UriNode(new Uri(obj))));
            }

            return triples;
        }
    }

    public Result<IEnumerable<Triple>> Run()
    {
        return SparqlServer.Query(this);
    }

    public async Task<Result<IEnumerable<Triple>>> RunAsync(CancellationToken cancellationToken = default)
    {
        return await SparqlServer.QueryAsync(this);
    }
}