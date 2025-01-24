using Microsoft.Extensions.DependencyInjection;
using FSR.DigitalTwin.App.Common.Middleware;
using FSR.DigitalTwin.Infra.ROS2;

namespace FSR.DigitalTwin.Infra.Common.Utils;

public static class DependencyInjection {
    public static void AddInfra(this IServiceCollection services) {
        // ROS2
        services.AddTransient<IRosWorkspace, RosWebSocketConnection>();
        // Insert more infrastructure if needed...
    }
}