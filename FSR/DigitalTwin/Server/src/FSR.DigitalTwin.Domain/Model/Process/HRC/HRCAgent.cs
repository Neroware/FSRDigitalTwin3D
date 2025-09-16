namespace FSR.DigitalTwin.Domain.Model.Process.HRC;

public enum EHRCAgentType
{
    Undefined, Human, Robot, WorkerOperator, Cobot
}

public class HRCAgent
{
    public required Resource Resource { init; get; }
    public required EHRCAgentType AgentType { init; get; }
    public string? Name { get; init; }

}