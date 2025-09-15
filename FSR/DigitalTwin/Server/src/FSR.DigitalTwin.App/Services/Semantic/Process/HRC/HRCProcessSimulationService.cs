using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.HRC;

public class HRCProcessSimulationService : IHRCProcessSimulationService
{
    private readonly ILogger<HRCProcessSimulationService> _logger;
    private readonly IHRCSimulationState _state;
    private static long _counter = 0;

    public HRCProcessSimulationService(IHRCSimulationState state, ILogger<HRCProcessSimulationService> logger)
    {
        _state = state ?? throw new ArgumentNullException(nameof(state));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public HRCProcessSimulationContext AddModel(Uri clientId, HRCModel model, string displayName, byte[]? data = null)
    {
        HRCProcessSimulationContext context = new() { Id = UriPrefix.PI + $"{clientId}.HRCProcessSimulation::{_counter++}", ClientId = clientId, DisplayName = displayName, Model = model, Data = data ?? [] };
        _state.Contexts.Add(context);
        return context;
    }

    public void AddLog(Uri id, HRCProcessSimulationLog log)
    {
        if (_state.Logs.TryGetValue(id, out List<HRCProcessSimulationLog>? value))
        {
            value.Add(log);
        }
        else
        {
            _state.Logs.Add(id, []);
        }
    }
}