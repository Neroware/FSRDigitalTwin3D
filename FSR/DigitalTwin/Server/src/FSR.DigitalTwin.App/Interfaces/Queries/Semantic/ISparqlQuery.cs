using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.Domain.SharedKernel;

namespace FSR.DigitalTwin.App.Interfaces.Queries.Semantic;

public interface ISparqlQuery<T> {

    ISparqlServer SparqlServer { init; get; }
    string Query { get; }

    Task<Result<T>> RunAsync(CancellationToken cancellationToken = default);
    Result<T> Run();

}