using VDS.RDF;

namespace FSR.DigitalTwin.App.Interfaces.Queries.Semantic;

public interface ISparqlResponseParser {
    IEnumerable<Triple> FromJson(string jsonResponse);
}