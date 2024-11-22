using FSR.DigitalTwin.App.Common.Network;
using FSR.DigitalTwin.App.Interfaces;
using FSR.DigitalTwin.App.Interfaces.Services;
using FSR.DigitalTwin.App.Services;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.DigitalTwin.App.Common.Utils;

public static class DependencyInjection {
    public static void AddAppServices(this IServiceCollection services) {
        services.AddSingleton<IConnectionState, ConnectionState>();
        services.AddSingleton<IOperationalState, OperationalState>();
        services.AddTransient<IDigitalTwinClientConnectionService, DigitalTwinClientConnectionService>();
        services.AddTransient<IRobotControlService, RosRobotService>();
    }
}