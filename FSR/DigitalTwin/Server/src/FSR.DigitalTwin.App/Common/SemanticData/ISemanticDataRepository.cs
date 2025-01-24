using FSR.DigitalTwin.Domain.SharedKernel;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Common.SemanticData;

public interface ISparqlServer
{
    Task<Result<IEnumerable<Triple>>> QueryAsync(string sparqlQuery, CancellationToken cancellationToken = default);
    Result<IEnumerable<Triple>> Query(string sparqlQuery);
}

public interface ISemanticGraphServer
{
    Task<Result<IGraph>> GetModelAsync(string graphUri, CancellationToken cancellationToken = default);
    Result<IGraph> GetModel(string graphUri);
}

public interface ITripletServer
{
    Task<Result<bool>> AddAsync(string subject, string predicate, string obj, CancellationToken cancellationToken = default);
    Result<bool> Add(string subject, string predicate, string obj);
    Task<Result<bool>> AddAllAsync(Tuple<string, string, string>[] rule, CancellationToken cancellationToken = default);
    Result<bool> AddAll(Tuple<string, string, string>[] rule);
}

public interface ISemanticDataRepository : ISparqlServer, ISemanticGraphServer, ITripletServer {
    // Intentionally left blank
}