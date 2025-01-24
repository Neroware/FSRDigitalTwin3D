using FSR.DigitalTwin.Infra.Interfaces;

namespace FSR.DigitalTwin.Infra.Jena;

public sealed class JenaHttpClient : IJenaHttpClient
{
    private readonly HttpClient _client;

    public JenaHttpClient(HttpClient client)
    {
        _client = client;
        if (_client.BaseAddress != null && _client.BaseAddress.Scheme == "fuseki")
        {
            var uriBuilder = new UriBuilder(_client.BaseAddress)
            {
                Scheme = "http", // or "https" if your Fuseki server uses HTTPS
                Port = 3030 // Remove the port if it's the default for the scheme
            };
            _client.BaseAddress = uriBuilder.Uri;
        }
    }

    public HttpResponseMessage Post(string url, StringContent content)
        => _client.PostAsync(new Uri(_client.BaseAddress ?? throw new NullReferenceException(nameof(_client.BaseAddress)), url), content, CancellationToken.None).Result;

    public async Task<HttpResponseMessage> PostAsync(string url, StringContent content, CancellationToken cancellationToken = default)
        => await _client.PostAsync(new Uri(_client.BaseAddress ?? throw new NullReferenceException(nameof(_client.BaseAddress)), url), content, cancellationToken);

}