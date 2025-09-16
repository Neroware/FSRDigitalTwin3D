using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process.HRC;

public interface IHRCKnowledgeService
{
    IEnumerable<Resource> GetInstances(Uri classRes);
    IEnumerable<Resource> GetIndividuals(Uri classRes);
    IEnumerable<Resource> GetProperty(Uri individual, Uri property);
    bool HasResourceType(Uri resource, Uri type);
    IDictionary<Resource, ISet<Resource>> RetrieveResourceStructure(Uri resource);
    IEnumerable<Resource> GetGoals();
    IEnumerable<Resource> GetCompoundGoals();
    IEnumerable<Resource> GetSubgoals();
    IEnumerable<Resource> GetBinaryResources();
    IEnumerable<Resource> GetAgents();
    IEnumerable<Resource> GetHumans();
    IEnumerable<Resource> GetCobots();
    IEnumerable<Resource> GetFunctions();
    IEnumerable<Resource> GetFunctionsByAgent(Uri agent);
    IEnumerable<IDictionary<Resource, IList<ISet<Resource>>>> GetDecompositionGraph(Uri goal);
    IDictionary<Resource, ISet<Resource>> GetDependencyGraph(Uri goal);
    IEnumerable<IEnumerable<Resource>> GetHierarchy(Uri goal);
    Resource GetResourceType(Uri resource);
    Resource GetFunctionTarget(Uri function);
    FunctionPropertyData GetFunctionDataProperties(Uri function);
    FunctionObjectData GetFunctionObjectProperties(Uri function);
}