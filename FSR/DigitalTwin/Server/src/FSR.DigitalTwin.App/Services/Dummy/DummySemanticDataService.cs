using FSR.DigitalTwin.App.Common.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Dummy;

namespace FSR.DigitalTwin.App.Services.Dummy;

public class DummySemanticDataService : IDummySemanticDataService
{
    private readonly ISemanticDataRepository _semanticDataRepository;

    public DummySemanticDataService(ISemanticDataRepository semanticDataRepository) {
        _semanticDataRepository = semanticDataRepository ?? throw new ArgumentNullException(nameof(semanticDataRepository));
    }

    public async Task PushDataAsync(string s, string p, string o)
    {
        await _semanticDataRepository.AddAsync(s, p, o);
    }
}