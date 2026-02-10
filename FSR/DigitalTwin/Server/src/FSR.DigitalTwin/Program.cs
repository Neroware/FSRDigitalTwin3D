// Comment out definitions if unwanted
#define ENABLE_ONTOLOGY_MODEL

using AasSecurity;
using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.App.Queries.Semantic.Base;
using FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Infra.Jena;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using VDS.RDF;
using INode = VDS.RDF.INode;

Console.WriteLine("AASX Server Core starting....");
var host = CreateHostBuilder(args).Build();

AasxServer.Program.Main(args);
SecurityHelper.SecurityInit();

#if ENABLE_ONTOLOGY_MODEL // Enable OWL ontology model
var ontoModel = host.Services.GetService<IOntologyModelService>()
    ?? throw new NullReferenceException("should not happen");
var ontoOptions = host.Services.GetRequiredService<IOptions<JenaSemanticDataRepositoryOptions>>().Value;
var knowledgeBase = host.Services.GetService<IHRCKnowledgeService>() ?? throw new NullReferenceException("should not happen");
if (knowledgeBase.GetGoals().IsNullOrEmpty())
{
    foreach (string modelFile in ontoOptions.ModelFiles)
    {
        string ontoModelPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), modelFile);
        if (!File.Exists(ontoModelPath))
            continue;
        await ontoModel.LoadOntologyModelAsync(ontoModelPath, OntologyModelFileFormat.RDF_XML);
    }
}
var doRotaryTable = knowledgeBase.GetDecompositionGraph(UriPrefix.PI + "task-assembly-goal");
var method = doRotaryTable.First();
foreach (ISet<Resource> rs in method[UriPrefix.PI + "doRotaryTable"])
{
    foreach (Resource r in rs)
    {
        Console.WriteLine($"n> {r}");
    }
}
// Console.WriteLine($"Number of triples loaded in semantic database: {ontoModel.Count}");
#endif

// Run file
await host.RunAsync();
await host.WaitForShutdownAsync();

static IHostBuilder CreateHostBuilder(string[] args) =>
    Host.CreateDefaultBuilder(args)
        .ConfigureWebHostDefaults(webBuilder =>
        {
            webBuilder.UseStartup<Startup>();
        });