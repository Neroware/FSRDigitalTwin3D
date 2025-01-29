namespace FSR.DigitalTwin.App.Interfaces.Services.Dummy;

public interface IDummySemanticDataService {
    public Task PushDataAsync(string s, string p, string o);
    public Task RunSubPredObjQuery();
}