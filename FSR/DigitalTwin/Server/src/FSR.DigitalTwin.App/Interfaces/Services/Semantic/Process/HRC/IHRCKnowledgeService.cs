using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;

public interface IHRCKnowledgeService
{
    IEnumerable<Resource> GetInstances(Resource classRes);
    IEnumerable<Resource> GetIndividuals(Resource classRes);
    IEnumerable<Resource> GetProperty(Resource individual, Resource property);
    bool HasResourceType(Resource resource, Resource type);
    IDictionary<Resource, ISet<Resource>> RetrieveResourceStructure(Resource resource);
    IEnumerable<Resource> GetGoals();
    IEnumerable<Resource> GetCompoundGoals();
    IEnumerable<Resource> GetSubgoals();
    IEnumerable<Resource> GetBinaryResources();
    IEnumerable<Resource> GetAgents();
    IEnumerable<Resource> GetHumans();
    IEnumerable<Resource> GetCobots();
    IEnumerable<Resource> GetFunctions();
    IEnumerable<Resource> GetFunctionsByAgent(Resource agent);
    IEnumerable<IDictionary<Resource, IList<ISet<Resource>>>> GetDecompositionGraph(Resource goal);
    IDictionary<Resource, ISet<Resource>> GetDependencyGraph(Resource goal);
    IEnumerable<IEnumerable<Resource>> GetHierarchy(Resource goal);
    Resource GetResourceType(Resource resource);
    Resource GetFunctionTarget(Resource function);
    FunctionPropertyData GetFunctionDataProperties(Resource function);
    FunctionObjectData GetFunctionObjectProperties(Resource function);
}