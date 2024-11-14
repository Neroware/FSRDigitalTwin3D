namespace FSR.DigitalTwin.App.Interfaces;

using ConnectonPair = Tuple<object, object>;

public interface IConnectionState {

    IDictionary<string, ConnectonPair> Known { get; init; }

}