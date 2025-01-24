using Microsoft.Extensions.DependencyInjection;
using FSR.DigitalTwin.App.Common.Middleware;
using FSR.DigitalTwin.Infra.ROS2;
using FSR.DigitalTwin.Infra.Jena;
using FSR.DigitalTwin.App.Common.SemanticData;
using FSR.DigitalTwin.Infra.Interfaces;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Text;

namespace FSR.DigitalTwin.Infra.Common.Utils;

public static class DependencyInjection {
    public static void AddInfra(this IServiceCollection services) {
        // ROS2
        services.AddOptions<RosWebSocketConnectionOptions>()
            .BindConfiguration(RosWebSocketConnectionOptions.ConfigurationSection)
            .ValidateDataAnnotations()
            .ValidateOnStart();
        services.AddTransient<IRosWorkspace, RosWebSocketConnection>();
        
        // Jena Database for semantic KG
        services.AddTransient<ISparqlServer, JenaSemanticDataRepository>();
        services.AddTransient<ISemanticGraphServer, JenaSemanticDataRepository>();
        services.AddTransient<ITripletServer, JenaSemanticDataRepository>();
        services.AddTransient<ISemanticDataRepository, JenaSemanticDataRepository>();
        services.AddHttpClient<IJenaHttpClient, JenaHttpClient>((ServiceProvider, httpClient) =>
        {
            var SemanticDataRepositoryOptions = ServiceProvider.GetRequiredService<IOptions<JenaSemanticDataRepositoryOptions>>().Value;

            httpClient.BaseAddress = new Uri(SemanticDataRepositoryOptions.BaseUrl);
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"admin:{SemanticDataRepositoryOptions.AccessToken}")));
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/sparql-results+json"));
        })
        .ConfigurePrimaryHttpMessageHandler(() =>
        {
            /// configure the primary Http-MesageHandler and...
            return new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(1)
            };
        })
        // Disable build in Lifetime
        .SetHandlerLifetime(Timeout.InfiniteTimeSpan);
        
        // Insert more infrastructure if needed...
    }
}