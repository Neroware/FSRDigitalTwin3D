namespace FSR.DigitalTwin.Domain.Model.Process;

public class AgentSkill
{
    public Resource Resource { get; }
    public Resource? Capability { set; get; }
    public List<Resource> Methods { set; get; } = [];
    public AgentSkill(Resource resource)
    {
        Resource = resource;
    }
}