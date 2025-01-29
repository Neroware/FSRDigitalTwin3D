using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.Domain.SharedKernel;

namespace FSR.DigitalTwin.App.Interfaces.Queries.Semantic;

public interface ISparqlQuery {
    ISparqlResponseParser Parser { get; }
    string Query { get; }
    ISparqlServer SparqlServer { init; get; }
}

public interface ISparqlQuery<T> : ISparqlQuery {

    Task<Result<T>> RunAsync(CancellationToken cancellationToken = default);
    Result<T> Run();

}