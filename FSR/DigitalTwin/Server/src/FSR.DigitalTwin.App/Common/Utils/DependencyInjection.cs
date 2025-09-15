using FSR.DigitalTwin.App.Interfaces;
using FSR.DigitalTwin.App.Interfaces.Services;
using FSR.DigitalTwin.App.Interfaces.Services.Dummy;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.App.Services;
using FSR.DigitalTwin.App.Services.Dummy;
using FSR.DigitalTwin.App.Services.Semantic;
using FSR.DigitalTwin.App.Services.Semantic.Process.HRC;
using Microsoft.Extensions.DependencyInjection;

namespace FSR.DigitalTwin.App.Common.Utils;

public static class DependencyInjection {
    public static void AddAppServices(this IServiceCollection services)
    {
        services.AddSingleton<IConnectionState, ConnectionState>();
        services.AddSingleton<IOperationalState, OperationalState>();
        services.AddTransient<IDigitalTwinClientConnectionService, DigitalTwinClientConnectionService>();
        services.AddTransient<IDummyRosService, DummyRosService>();
        services.AddTransient<IDummySemanticDataService, DummySemanticDataService>();
        services.AddTransient<IOntologyModelService, OntologyModelService>();
        services.AddTransient<IHRCKnowledgeService, ProductionKnowledgeService>();
        services.AddTransient<IHRCKnowledgeAuthoringService, HRCKnowledgeAuthoringService>();
        services.AddSingleton<IHRCSimulationState, HRCSimulationState>();
        services.AddTransient<IHRCProcessSimulationService, HRCProcessSimulationService>();
    }
}