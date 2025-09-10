using FSR.DigitalTwin.App.Common.Utils.Semantic;
using FSR.DigitalTwin.App.Ex;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic;
using FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;
using FSR.DigitalTwin.App.Queries.Semantic.Base;
using FSR.DigitalTwin.App.Queries.Semantic.Process.HRC;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;
using Microsoft.Extensions.Logging;
using VDS.RDF;

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

    public IEnumerable<INode> GetAgents()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "AutonomousAgent") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetBinaryResources()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<INode> GetCobots()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "Cobot") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetCompoundGoals()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "CompoundProductionGoal") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<IDictionary<INode, IList<ISet<INode>>>> GetDecompositionGraph(Uri prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new HRCKnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        List<IDictionary<INode, IList<ISet<INode>>>> graphs = [];
        var methods = GetProperty(prodGoal, UriPrefix.DUL + "hasConstituent")
            .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
        foreach (var method in methods)
        {
            if (HasResourceType(method.Uri, UriPrefix.SOHO + "ProductionMethod"))
            {
                Dictionary<INode, IList<ISet<INode>>> methodGraph = [];
                var tasks = GetProperty(method.Uri, UriPrefix.DUL + "hasConstituent")
                    .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
                foreach (var task in tasks)
                {
                    if (HasResourceType(task.Uri, UriPrefix.SOHO + "ProductionTask"))
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

    public IDictionary<INode, ISet<INode>> GetDependencyGraph(Uri prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new HRCKnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        Dictionary<INode, ISet<INode>> depGraph = [];
        var graphs = GetDecompositionGraph(prodGoal);

        foreach (var graph in graphs)
        {
            foreach (INode key in graph.Keys.ToHashSet())
            {
                if (!depGraph.ContainsKey(key))
                {
                    depGraph.Add(key, new HashSet<INode>());
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

    public FunctionPropertyData GetFunctionDataProperties(Uri function)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetFunctionPropertyDataQuery(function) { SparqlServer = server });
        if (result.IsFailure)
        {
            return new FunctionPropertyData()
            {
                Function = new UriNode(function),
                ProcedureId = null,
                ProcedureName = null,
                ProcedureDescription = null,
                Duration = 0,
                DurationUncertainty = 0
            };
        }
        return result.Value;
    }

    public FunctionObjectData GetFunctionObjectProperties(Uri function)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetFunctionObjectDataQuery(function) { SparqlServer = server });
        if (result.IsFailure)
        {
            return new FunctionObjectData()
            {
                Function = new UriNode(function),
                Target = [],
                StartLocation = [],
                EndLocation = [],
                Location = []
            };
        }
        return result.Value;
    }

    public IEnumerable<INode> GetFunctions()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "Function") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetFunctionsByAgent(Uri agent)
    {
        return GetProperty(agent, UriPrefix.SOHO + "canPerform");
    }

    public INode GetFunctionTarget(Uri function)
    {
        var functionObjects = GetFunctionObjectProperties(function);
        if (functionObjects.Target.Count == 0)
        {
            throw new HRCKnowledgeException($"Missing SOHO:hasTarget property for function: {function}");
        }
        return functionObjects.Target.First();
    }

    public IEnumerable<INode> GetHumans()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "WorkerOperator") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetIndividuals(Uri classRes)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetIndividualsQuery(classRes) { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetInstances(Uri classRes)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(classRes) { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<INode> GetGoals()
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(UriPrefix.SOHO + "ProductionGoal") { SparqlServer = server });
        return result.IsSuccess ? result.Value.Distinct() : [];
    }

    public IEnumerable<IEnumerable<INode>> GetHierarchy(Uri prodGoal)
    {
        if (!HasResourceType(prodGoal, UriPrefix.SOHO + "ProductionGoal"))
        {
            throw new HRCKnowledgeException($"Wrong parameter type, a resource/individual of type <{UriPrefix.SOHO}ProductionGoal> expected, received <{prodGoal}> of other type!");
        }
        var graph = GetDependencyGraph(prodGoal);
        Dictionary<INode, ISet<INode>> copy = new(graph);
        foreach (UriNode task in copy.Keys.Where(k => k.NodeType == NodeType.Uri).Cast<UriNode>().ToHashSet())
        {
            if (HasResourceType(task.Uri, UriPrefix.SOHO + "Function") &&
                    !copy[task].Any())
            {
                graph.Remove(task);
                foreach (UriNode key in graph.Keys.Where(k => k.NodeType == NodeType.Uri).Cast<UriNode>().ToHashSet())
                {
                    graph[key].Remove(task);
                }
            }
        }

        Dictionary<INode, ISet<INode>> incidenceGraph = [];
        HashSet<INode> visited = [];
        foreach (INode from in graph.Keys.ToHashSet())
        {
            visited.Add(from);
            foreach (INode to in graph[from])
            {
                visited.Add(to);
                if (!incidenceGraph.ContainsKey(to))
                {
                    incidenceGraph.Add(to, new HashSet<INode>());
                }
                incidenceGraph[to].Add(from);
            }
        }

        foreach (INode node in visited)
        {
            if (!incidenceGraph.ContainsKey(node))
            {
                incidenceGraph.Add(node, new HashSet<INode>());
            }
        }

        return RunTopolicalSort(incidenceGraph);
    }

    public IEnumerable<INode> GetSubgoals()
    {
        throw new NotImplementedException();
    }

    public IEnumerable<INode> GetProperty(Uri individual, Uri property)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetPropertyQuery(property, individual) { SparqlServer = server });
        return result.IsSuccess ? result.Value : [];
    }

    public INode GetResourceType(Uri resource)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetResourceTypeQuery(resource) { SparqlServer = server });
        if (result.IsFailure || !result.Value.Any())
        {
            throw new HRCKnowledgeException($"Missing RDF:type property for resource: {resource}");
        }
        return result.Value.First();
    }

    public bool HasResourceType(Uri resource, Uri type)
    {
        var result = _ontology.RunSparqlQuery((server) =>
            new GetInstancesQuery(type) { SparqlServer = server });
        return result.IsSuccess && result.Value.Where(n => n.NodeType == NodeType.Uri)
            .Cast<UriNode>().Contains(new UriNode(resource));
    }

    public IDictionary<INode, ISet<INode>> RetrieveResourceStructure(Uri resource)
    {
        Dictionary<INode, ISet<INode>> structure = [];
        RetrieveResourceStructure(resource, structure);
        return structure;
    }

    private void RetrieveResourceStructure(Uri resource, Dictionary<INode, ISet<INode>> subTree)
    {
        var hasConstituent = GetProperty(resource, UriPrefix.DUL + "hasConstituent");
        if (hasConstituent == null || !hasConstituent.Any())
        {
            subTree.Add(new UriNode(resource), new HashSet<INode>());
        }
        else
        {
            HashSet<INode> children = [];
            foreach (var child in hasConstituent.Where(child => child.NodeType == NodeType.Uri).Cast<UriNode>())
            {
                children.Add(child);
                RetrieveResourceStructure(child.Uri, subTree);
            }
            subTree.Add(new UriNode(resource), children);
        }
    }

    // TODO Very expensive, maybe translate to SPARQL query eventually...
    private void RetrieveProductionTaskDecomposition(UriNode task, Dictionary<INode, IList<ISet<INode>>> graph)
    {
        var prop = GetProperty(task.Uri, UriPrefix.DUL + "hasConstituent")
            .Where(n => n.NodeType == NodeType.Uri).Cast<UriNode>();
        if (HasResourceType(task.Uri, UriPrefix.SOHO + "DisjunctiveComplexTask"))
        {
            foreach (var subTask in prop)
            {
                if (HasResourceType(subTask.Uri, UriPrefix.SOHO + "ProductionTask"))
                {
                    HashSet<INode> disjunction = [];
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
            HashSet<INode> subTasks = [];
            foreach (var subTask in prop)
            {
                if (HasResourceType(subTask.Uri, UriPrefix.SOHO + "ProductionTask"))
                {
                    subTasks.Add(subTask);
                    if (!graph.ContainsKey(subTask))
                    {
                        graph.Add(subTask, []);
                    }
                    if (!HasResourceType(subTask.Uri, UriPrefix.SOHO + "Function"))
                    {
                        RetrieveProductionTaskDecomposition(subTask, graph);
                    }
                }
            }

            graph[task].Add(subTasks);
        }
    }

    private static List<IList<INode>> RunTopolicalSort(Dictionary<INode, ISet<INode>> dependencies)
    {
        Dictionary<INode, ISet<INode>> graph = new(dependencies);
        List<INode> sorted = [];
        foreach (INode key in graph.Keys.ToHashSet()) {
            if (!graph[key].Any()) {
                sorted.Add(key);
            }
        }

        List<IList<INode>> hierarchy = [];
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
                foreach (INode key in graph.Keys.ToHashSet()) {
                    graph[key].Remove(res);
                }
            }

            sorted.Clear();
            topLevel++;

            foreach (INode key in graph.Keys.ToHashSet()) {
                if (!graph[key].Any()) {
                    sorted.Add(key);
                }
            }
        }

        return hierarchy;
    }
}