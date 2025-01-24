using System.Text;
using FSR.DigitalTwin.App.Common.SemanticData;
using FSR.DigitalTwin.Domain.SharedKernel;
using FSR.DigitalTwin.Infra.Interfaces;
using Microsoft.Extensions.Logging;
using Polly;
using Polly.Retry;
using VDS.RDF;

namespace FSR.DigitalTwin.Infra.Jena;

public class JenaSemanticDataRepository : ISemanticDataRepository
{
    private readonly IJenaHttpClient _jenaHttpClient;
    private readonly ILogger<JenaSemanticDataRepository> _logger;
    private readonly IAsyncPolicy _retryPolicy;
    // private readonly AsyncRetryPolicy<HttpResponseMessage> _asyncRetryPolicy;

    public JenaSemanticDataRepository(IJenaHttpClient jenaHttpClient, ILogger<JenaSemanticDataRepository> logger) {
        _jenaHttpClient = jenaHttpClient ?? throw new ArgumentNullException(nameof(jenaHttpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _retryPolicy = Policy
            .Handle<HttpRequestException>()
            .WaitAndRetryAsync(3, retryAttempt => 
                TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
    }

    public Result<bool> Add(string subject, string predicate, string obj)
    {
        throw new NotImplementedException();
    }

    public Result<bool> AddAll(Tuple<string, string, string>[] rule)
    {
        throw new NotImplementedException();
    }

    public Task<Result<bool>> AddAllAsync(Tuple<string, string, string>[] rule, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public async Task<Result<bool>> AddAsync(string subject, string predicate, string obj, CancellationToken cancellationToken = default)
    {
        try
        {
            return await _retryPolicy.ExecuteAsync(async () =>
            {
                var triple = $"<{subject}> <{predicate}> <{obj}> .";
                //"text/turtle" for Turtle format RDF data
                var content = new StringContent(triple, Encoding.UTF8, "text/turtle");

                //Endpoint: The correct endpoint for adding data is /data.
                //Query Parameter: The ? default query parameter specifies that you're adding to the default graph.
                //If you want to add to a specific named graph, you would use ?graph=URI instead.
                var response = await _jenaHttpClient.PostAsync("pddtriples/data", content, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    return Result.Failure<bool>($"Failed to add triple. Status code: {response.StatusCode}");
                }

                return Result.Success(true);
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while adding triple: {Subject} {Predicate} {Object}", subject, predicate, obj);
            return Result.Failure<bool>(ex.Message);
        }
    }

    public Result<IGraph> GetModel(string graphUri)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IGraph>> GetModelAsync(string graphUri, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }

    public Result<IEnumerable<Triple>> Query(string sparqlQuery)
    {
        throw new NotImplementedException();
    }

    public Task<Result<IEnumerable<Triple>>> QueryAsync(string sparqlQuery, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException();
    }
}