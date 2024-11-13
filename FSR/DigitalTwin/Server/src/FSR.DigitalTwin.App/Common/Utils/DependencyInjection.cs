using FSR.DigitalTwin.App.Interfaces.Services;
using FSR.DigitalTwin.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.DigitalTwin.App.Common.Utils;

public static class DependencyInjection {
    public static void AddAppServices(this IServiceCollection services) {
        services.AddSingleton<IDigitalTwinClientConnectionService, DigitalTwinClientConnectionService>();
        // services.AddTransient<IDigitalTwinOperationalService, DigitalTwinOperationalService>();
    }
}