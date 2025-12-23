using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.SharedKernel;


namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic;

public interface IOntologyModelService
{
    bool Empty => IsEmpty();
    int Count => CountTriples();

    Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default);
    bool IsEmpty();
    Task<int> CountTriplesAsync(CancellationToken cancellationToken = default);
    int CountTriples();

    Task<bool> LoadOntologyModelAsync(string ontoFile, OntologyModelFileFormat ontoFormat = OntologyModelFileFormat.DEFAULT, CancellationToken cancellationToken = default);
    Task<bool> DeleteOntologyModelAsync(CancellationToken cancellationToken = default);

    Result<T> RunSparqlQuery<T>(Func<ISparqlServer, ISparqlQuery<T>> queryFactory);
    Task<Result<T>> RunSparqlQueryAsync<T>(Func<ISparqlServer, ISparqlQuery<T>> queryFactory, CancellationToken cancellationToken = default);

    IEnumerable<Resource> GetInstances(Resource classRes);
    IEnumerable<Resource> GetIndividuals(Resource classRes);
    IEnumerable<Resource> GetProperty(Resource individual, Resource property);
    IEnumerable<Resource> GetResourceType(Resource resource);
}

public enum OntologyModelFileFormat {
    DEFAULT = 0,
    TURTLE = 1,
    RDF_XML = 2
}