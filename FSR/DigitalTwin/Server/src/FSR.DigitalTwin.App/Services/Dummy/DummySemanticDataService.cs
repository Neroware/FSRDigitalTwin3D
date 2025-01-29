using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Dummy;
using FSR.DigitalTwin.App.Queries.Semantic.Example;
using Microsoft.Extensions.Logging;
using VDS.RDF;

namespace FSR.DigitalTwin.App.Services.Dummy;

public class DummySemanticDataService : IDummySemanticDataService
{
    private readonly ISemanticDataRepository _semanticDataRepository;
    private readonly ILogger<IDummySemanticDataService> _logger;

    public DummySemanticDataService(ISemanticDataRepository semanticDataRepository, ILogger<DummySemanticDataService> logger) {
        _semanticDataRepository = semanticDataRepository ?? throw new ArgumentNullException(nameof(semanticDataRepository));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task PushDataAsync(string s, string p, string o)
    {
        await _semanticDataRepository.AddAsync(s, p, o);
    }

    public async Task RunSubPredObjQuery()
    {
        GetAllTripletsQuery query = new(_semanticDataRepository);
        var result = await query.RunAsync();
        foreach (Triple triple in result.Value) {
            _logger.LogInformation("Found triple: {Triple}", triple);
        }
    }
}