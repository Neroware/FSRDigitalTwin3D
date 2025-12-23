using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Ex;
using FSR.DigitalTwin.App.Interfaces.Queries.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Queries.Semantic.Base;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.SharedKernel;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services.Semantic;

public class OntologyModelService : IOntologyModelService
{
    private readonly ITripletServer _tripletServer;
    private readonly ISparqlServer _sparqlServer;
    private readonly ILogger<OntologyModelService> _logger;

    public OntologyModelService(ITripletServer tripletServer, ISparqlServer sparqlServer, ILogger<OntologyModelService> logger)
    {
        _tripletServer = tripletServer ?? throw new ArgumentNullException(nameof(tripletServer));
        _sparqlServer = sparqlServer ?? throw new ArgumentNullException(nameof(sparqlServer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public int CountTriples()
    {
        GetTripleCountQuery tripleCountQuery = new(_sparqlServer);
        var tripleCountQueryResponse = tripleCountQuery.Run();
        return tripleCountQueryResponse.IsFailure ? -1 : tripleCountQueryResponse.Value;
    }

    public async Task<int> CountTriplesAsync(CancellationToken cancellationToken = default)
    {
        GetTripleCountQuery tripleCountQuery = new(_sparqlServer);
        var tripleCountQueryResponse = await tripleCountQuery.RunAsync(cancellationToken);
        return tripleCountQueryResponse.IsFailure ? -1 : tripleCountQueryResponse.Value;
    }

    public async Task<bool> DeleteOntologyModelAsync(CancellationToken cancellationToken = default)
    {
        var result = await _tripletServer.DeleteAllAsync(cancellationToken);
        return result.IsSuccess;
    }

    public bool IsEmpty()
    {
        GetTripleCountQuery tripleCountQuery = new(_sparqlServer);
        var tripleCountQueryResponse = tripleCountQuery.Run();
        return tripleCountQueryResponse.IsFailure || tripleCountQueryResponse.Value == 0;
    }

    public async Task<bool> IsEmptyAsync(CancellationToken cancellationToken = default)
    {
        GetTripleCountQuery tripleCountQuery = new(_sparqlServer);
        var tripleCountQueryResponse = await tripleCountQuery.RunAsync(cancellationToken);
        return tripleCountQueryResponse.IsFailure || tripleCountQueryResponse.Value == 0;
    }

    public async Task<bool> LoadOntologyModelAsync(string ontoFile, OntologyModelFileFormat ontoFormat = OntologyModelFileFormat.DEFAULT, CancellationToken cancellationToken = default)
    {
        if (!File.Exists(ontoFile))
        {
            _logger.LogError("File not found: {FilePath}", ontoFile);
            return false;
        }
        string ontoFormatStr = GetFormatString(ontoFormat);
        var result = await _tripletServer.LoadFileAsync(ontoFile, ontoFormatStr, cancellationToken);
        return result.IsSuccess;
    }

    private static string GetFormatString(OntologyModelFileFormat format) => format switch
    {
        OntologyModelFileFormat.DEFAULT or OntologyModelFileFormat.TURTLE => "text/turtle",
        OntologyModelFileFormat.RDF_XML => "application/rdf+xml",
        _ => GetFormatString(OntologyModelFileFormat.TURTLE)
    };
    
    public Result<T> RunSparqlQuery<T>(Func<ISparqlServer, ISparqlQuery<T>> queryFactory)
    {
        ISparqlQuery<T> query = queryFactory(_sparqlServer);
        return query.Run();
    }

    public Task<Result<T>> RunSparqlQueryAsync<T>(Func<ISparqlServer, ISparqlQuery<T>> queryFactory, CancellationToken cancellationToken = default)
    {
        ISparqlQuery<T> query = queryFactory(_sparqlServer);
        return query.RunAsync(cancellationToken);
    }

    public IEnumerable<Resource> GetIndividuals(Resource classRes)
    {
        var result = RunSparqlQuery((server) =>
            new GetIndividualsQuery(classRes) { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<Resource> GetInstances(Resource classRes)
    {
        var result = RunSparqlQuery((server) =>
            new GetInstancesQuery(classRes) { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<Resource> GetProperty(Resource individual, Resource property)
    {
        var result = RunSparqlQuery((server) =>
            new GetPropertyQuery(property, individual) { SparqlServer = server });
        return result.IsSuccess ? result.Value : [];
    }

    public IEnumerable<Resource> GetResourceType(Resource resource)
    {
        var result = RunSparqlQuery((server) =>
            new GetResourceTypeQuery(resource) { SparqlServer = server });
        if (result.IsFailure)
        {
            throw new KnowledgeException($"Failed to retreive types of {resource}");
        }
        return result.Value;
    }
}