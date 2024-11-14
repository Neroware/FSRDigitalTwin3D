using AasxServerStandardBib.Logging;
using FSR.DigitalTwin.App.Interfaces;
using FSR.DigitalTwin.App.Interfaces.Services;
using ConnectonPair = System.Tuple<object, object>;

namespace FSR.DigitalTwin.App.Services;

public class DigitalTwinClientConnectionService : IDigitalTwinClientConnectionService
{
    private readonly IAppLogger<DigitalTwinClientConnectionService> _logger;
    private readonly IConnectionState _connectionState;

    public DigitalTwinClientConnectionService(IAppLogger<DigitalTwinClientConnectionService> logger, IConnectionState connectionState) {
        _logger = logger ?? throw new NullReferenceException(nameof(logger));
        _connectionState = connectionState ?? throw new NullReferenceException(nameof(connectionState));
    }

    public bool AddBidirectionalConnectionStream(string id, object reader, object writer)
    {
        _logger.LogDebug($"Added bidirectional connection with id = {id}.");
        if (_connectionState.Known.ContainsKey(id)) {
            return false;
        }
        _connectionState.Known.Add(id, new ConnectonPair(reader, writer));
        return true;
    }

    public ConnectonPair[] GetAllConnections()
    {
        return [.. _connectionState.Known.Values];
    }

    public ConnectonPair GetConnectionById(string id)
    {
        return _connectionState.Known[id];
    }

    public ConnectonPair? TryGetConnectionById(string id){
        _connectionState.Known.TryGetValue(id, out ConnectonPair? value);
        return value;
    }

    public bool RemoveConnection(string id)
    {
        return _connectionState.Known.Remove(id);
    }
}