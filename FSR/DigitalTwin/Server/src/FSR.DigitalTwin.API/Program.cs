//var builder = WebApplication.CreateBuilder(args);
//var app = builder.Build();

//app.MapGet("/", () => "Hello World!");

//app.Run();

using AasSecurity;

Console.WriteLine("AASX Server Core starting....");
var host = CreateHostBuilder(args).Build();

AasxServer.Program.Main(args);
SecurityHelper.SecurityInit();

// Launch the Dummy ROS2 Service
var serviceProvider = host.Services;
var robotControls = serviceProvider.GetService<FSR.DigitalTwin.App.Interfaces.Services.Dummy.IDummyRosService>() 
    ?? throw new NullReferenceException("should not happen");
robotControls.RunTest();

// Launch the Dummy Semantic Repo Service
var semanticRepo = serviceProvider.GetService<FSR.DigitalTwin.App.Interfaces.Services.Dummy.IDummySemanticDataService>() 
    ?? throw new NullReferenceException("should not happten");
await semanticRepo.PushDataAsync("pi:dog", "pi:inherits", "pi:animal");

// Run file
await host.RunAsync();
host.WaitForShutdownAsync();


static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });
