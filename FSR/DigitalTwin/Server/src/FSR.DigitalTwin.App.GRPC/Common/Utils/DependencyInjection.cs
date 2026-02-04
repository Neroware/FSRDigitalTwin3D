using FSR.DigitalTwin.App.GRPC.Services;
using FSR.DigitalTwin.App.GRPC.Services.RPC;
using FSR.DigitalTwin.App.Interfaces.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.DigitalTwin.App.GRPC.Common.Utils;

public static class GrpcService {
    public static void MapAppGrpcServices(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGrpcService<AssetAdministrationShellRepositoryRpcService>();
        endpoints.MapGrpcService<SubmodelRepositoryRpcService>();
        endpoints.MapGrpcService<AssetAdministrationShellRpcService>();
        endpoints.MapGrpcService<SubmodelRpcService>();
        endpoints.MapGrpcService<DigitalTwinClientConnectionRpcService>();
        endpoints.MapGrpcService<HRCProcessSimulationRpcService>();
        endpoints.MapGrpcService<BDIDecisionProcessRpcService>();
    }

    public static void AddAppGrpcServices(this IServiceCollection services) {
        services.AddTransient<IDigitalTwinOperationalService, DigitalTwinOperationalService>();
    }
}