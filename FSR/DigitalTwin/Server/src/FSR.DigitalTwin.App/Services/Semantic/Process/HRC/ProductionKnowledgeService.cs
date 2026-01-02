using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Ex;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.App.Queries.Semantic.Base;
using FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using Microsoft.Extensions.Logging;

namespace FSR.DigitalTwin.App.Services.Semantic.Process.HRC;

public class ProductionKnowledgeService : IHRCKnowledgeService
{
    private readonly IOntologyModelService _ontology;
    private readonly ILogger<ProductionKnowledgeService> _logger;

    public ProductionKnowledgeService(IOntologyModelService ontology, ILogger<ProductionKnowledgeService> logger)
    {
        _ontology = ontology ?? throw new ArgumentNullException(nameof(ontology));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<Resource> GetAgents()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "AutonomousAgent") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<Resource> GetBinaryResources()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<Resource> GetCobots()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "Cobot") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<Resource> GetCompoundGoals()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "CompoundProductionGoal") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<IDictionary<Resource, IList<ISet<Resource>>>> GetDecompositionGraph(Resource prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new KnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        List<IDictionary<Resource, IList<ISet<Resource>>>> graphs = [];
        var methods = _ontology.GetProperty(prodGoal, UriPrefix.DUL + "hasConstituent");
        foreach (var method in methods)
        {
            if (HasResourceType(method, UriPrefix.SOHO + "ProductionMethod"))
            {
                Dictionary<Resource, IList<ISet<Resource>>> methodGraph = [];
                var tasks = _ontology.GetProperty(method, UriPrefix.DUL + "hasConstituent");
                foreach (var task in tasks)
                {
                    if (HasResourceType(task, UriPrefix.SOHO + "ProductionTask"))
                    {
                        if (!methodGraph.ContainsKey(task))
                        {
                            methodGraph.Add(task, []);
                        }

                        RetrieveProductionTaskDecomposition(task, methodGraph);
                    }
                }

                graphs.Add(methodGraph);
            }
        }

        return graphs;
    }

    public IDictionary<Resource, ISet<Resource>> GetDependencyGraph(Resource prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new KnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        Dictionary<Resource, ISet<Resource>> depGraph = [];
        var graphs = GetDecompositionGraph(prodGoal);

        foreach (var graph in graphs)
        {
            foreach (Resource key in graph.Keys.ToHashSet())
            {
                if (!depGraph.ContainsKey(key))
                {
                    depGraph.Add(key, new HashSet<Resource>());
                }
                foreach (var subTasks in graph[key])
                {
                    foreach (var subTask in subTasks)
                    {
                        depGraph[key].Add(subTask);
                    }
                }
            }
        }

        return depGraph;
    }

    public FunctionPropertyData GetFunctionDataProperties(Resource function)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetFunctionPropertyDataQuery(function) { SparqlServer = server });
        if (result.IsFailure)
        {
            return new FunctionPropertyData()
            {
                Function = function,
                ProcedureId = -1,
                ProcedureName = null,
                ProcedureDescription = null,
                Duration = 0,
                DurationUncertainty = 0
            };
        }
        return result.Value;
    }

    public FunctionObjectData GetFunctionObjectProperties(Resource function)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetFunctionObjectDataQuery(function) { SparqlServer = server });
        if (result.IsFailure)
        {
            return new FunctionObjectData()
            {
                Function = function,
                Target = [],
                StartLocation = [],
                EndLocation = [],
                Location = []
            };
        }
        return result.Value;
    }

    public IEnumerable<Resource> GetFunctions()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "Function") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<Individual> GetFunctionsByAgent(Resource agent)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetFunctionsByAgentQuery(agent) { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public Resource GetFunctionTarget(Resource function)
    {
        var functionObjects = GetFunctionObjectProperties(function);
        if (functionObjects.Target.Count == 0)
        {
            throw new KnowledgeException($"Missing SOHO:hasTarget property for function: {function}");
        }
        return functionObjects.Target.First();
    }

    public IEnumerable<Resource> GetHumans()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "WorkOperator") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public InteractionModality? GetInteractionModality(Resource task)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInteractionModality(task) { SparqlServer = server });
        return result.IsSuccess ? result.Value : null;
    }

    public IEnumerable<Resource> GetGoals()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "ProductionGoal") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<IEnumerable<Resource>> GetHierarchy(Resource prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new KnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        var graph = GetDependencyGraph(prodGoal);
        Dictionary<Resource, ISet<Resource>> copy = new(graph);
        foreach (Resource task in copy.Keys.ToHashSet())
        {
            if (HasResourceType(task, UriPrefix.SOHO + "Function") &&
                    !copy[task].Any())
            {
                graph.Remove(task);
                foreach (Resource key in graph.Keys.ToHashSet())
                {
                    graph[key].Remove(task);
                }
            }
        }

        Dictionary<Resource, ISet<Resource>> incidenceGraph = [];
        HashSet<Resource> visited = [];
        foreach (Resource from in graph.Keys.ToHashSet())
        {
            visited.Add(from);
            foreach (Resource to in graph[from])
            {
                visited.Add(to);
                if (!incidenceGraph.ContainsKey(to))
                {
                    incidenceGraph.Add(to, new HashSet<Resource>());
                }
                incidenceGraph[to].Add(from);
            }
        }

        foreach (Resource node in visited)
        {
            if (!incidenceGraph.ContainsKey(node))
            {
                incidenceGraph.Add(node, new HashSet<Resource>());
            }
        }

        return RunTopolicalSort(incidenceGraph);
    }

    public IEnumerable<Resource> GetSubgoals()
    {
        throw new NotImplementedException();
    }

    private bool HasResourceType(Resource resource, Resource type)
    {
        var result = _ontology.GetInstances(type);
        return result?.Contains(resource) ?? false;
    }

    public IDictionary<Resource, ISet<Resource>> RetrieveResourceStructure(Resource resource)
    {
        Dictionary<Resource, ISet<Resource>> structure = [];
        RetrieveResourceStructure(resource, structure);
        return structure;
    }

    private void RetrieveResourceStructure(Resource resource, Dictionary<Resource, ISet<Resource>> subTree)
    {
        var hasConstituent = _ontology.GetProperty(resource, UriPrefix.DUL + "hasConstituent");
        if (hasConstituent == null || !hasConstituent.Any())
        {
            subTree.Add(resource, new HashSet<Resource>());
        }
        else
        {
            HashSet<Resource> children = [];
            foreach (var child in hasConstituent)
            {
                children.Add(child);
                RetrieveResourceStructure(child, subTree);
            }
            subTree.Add(resource, children);
        }
    }

    // TODO Very expensive, maybe translate to SPARQL query eventually...
    private void RetrieveProductionTaskDecomposition(Resource task, Dictionary<Resource, IList<ISet<Resource>>> graph)
    {
        var prop = _ontology.GetProperty(task, UriPrefix.DUL + "hasConstituent");
        if (HasResourceType(task, UriPrefix.SOHO + "DisjunctiveComplexTask"))
        {
            foreach (var subTask in prop)
            {
                if (HasResourceType(subTask, UriPrefix.SOHO + "ProductionTask"))
                {
                    HashSet<Resource> disjunction = [];
                    disjunction.Add(subTask);
                    graph[task].Add(disjunction);

                    if (!graph.ContainsKey(subTask))
                    {
                        graph.Add(subTask, []);
                    }

                    RetrieveProductionTaskDecomposition(subTask, graph);
                }
            }
        }
        else
        {
            HashSet<Resource> subTasks = [];
            foreach (var subTask in prop)
            {
                if (HasResourceType(subTask, UriPrefix.SOHO + "ProductionTask"))
                {
                    subTasks.Add(subTask);
                    if (!graph.ContainsKey(subTask))
                    {
                        graph.Add(subTask, []);
                    }
                    if (!HasResourceType(subTask, UriPrefix.SOHO + "Function"))
                    {
                        RetrieveProductionTaskDecomposition(subTask, graph);
                    }
                }
            }

            graph[task].Add(subTasks);
        }
    }

    private static List<IList<Resource>> RunTopolicalSort(Dictionary<Resource, ISet<Resource>> dependencies)
    {
        Dictionary<Resource, ISet<Resource>> graph = new(dependencies);
        List<Resource> sorted = [];
        foreach (Resource key in graph.Keys.ToHashSet()) {
            if (!graph[key].Any()) {
                sorted.Add(key);
            }
        }

        List<IList<Resource>> hierarchy = [];
        int topLevel = 0;
        while (sorted.Count != 0)
        {
            foreach (var res in sorted)
            {
                if (hierarchy.Count <= topLevel) {
                    hierarchy.Add([]);
                }
                hierarchy[topLevel].Add(res);
                graph.Remove(res);
                foreach (Resource key in graph.Keys.ToHashSet()) {
                    graph[key].Remove(res);
                }
            }

            sorted.Clear();
            topLevel++;

            foreach (Resource key in graph.Keys.ToHashSet()) {
                if (!graph[key].Any()) {
                    sorted.Add(key);
                }
            }
        }

        return hierarchy;
    }
}