namespace FSR.DigitalTwin.App.Interfaces.Services;

public interface IDummySemanticDataService {
    public Task PushDataAsync(string s, string p, string o);
}