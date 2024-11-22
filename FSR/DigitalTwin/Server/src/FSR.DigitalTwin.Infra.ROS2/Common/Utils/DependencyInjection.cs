using FSR.DigitalTwin.App.Common.Network;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.DigitalTwin.Infra.ROS2.Common.Utils;

public static class DependencyInjection {
    public static void AddInfraRos2(this IServiceCollection services) {
        services.AddTransient<IRosConnection, RosWebSocketConnection>();
    }
}