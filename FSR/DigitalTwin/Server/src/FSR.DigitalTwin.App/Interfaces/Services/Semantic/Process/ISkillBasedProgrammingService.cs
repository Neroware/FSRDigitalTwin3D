using FSR.DigitalTwin.Domain.Model;
using FSR.DigitalTwin.Domain.Model.Process;

namespace FSR.DigitalTwin.App.Interfaces.Services.Semantic.Process;

public interface ISkillBasedProgrammingService
{
    public IEnumerable<AgentSkill> GetSkills();
    public IEnumerable<Resource> GetCapabilities(Resource agent);
    public IEnumerable<Resource> GetAgents();
    public IEnumerable<Resource> GetDevicePrimitives(Resource agentSkill);
}