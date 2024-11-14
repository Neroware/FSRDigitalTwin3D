using FSR.DigitalTwin.App.Interfaces;

namespace FSR.DigitalTwin.App;

public class ConnectionState : IConnectionState
{
    public IDictionary<string, Tuple<object, object>> Known { get; init; } = new Dictionary<string, Tuple<object, object>>();
}