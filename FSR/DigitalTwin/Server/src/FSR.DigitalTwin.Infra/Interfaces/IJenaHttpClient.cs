namespace FSR.DigitalTwin.Infra.Interfaces;

public interface IJenaHttpClient {
    Task<HttpResponseMessage> PostAsync(string url, StringContent content, CancellationToken cancellationToken = default);
    HttpResponseMessage Post(string url, StringContent content);
}