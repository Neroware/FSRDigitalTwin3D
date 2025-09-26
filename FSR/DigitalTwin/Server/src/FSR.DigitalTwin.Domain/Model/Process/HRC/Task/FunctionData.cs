namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public record FunctionPropertyData
{
    public required Resource Function { init; get; }
    public string? ProcedureName { init; get; }
    public string? ProcedureDescription { init; get; }
    public string? ProcedureId { init; get; }
    public float Duration { init; get; }
    public float DurationUncertainty { init; get; }
}

public record FunctionObjectData
{
    public required Resource Function { init; get; }
    public HashSet<Resource> Target { init; get; } = [];
    public HashSet<Resource> StartLocation { init; get; } = [];
    public HashSet<Resource> EndLocation { init; get; } = [];
    public HashSet<Resource> Location { init; get; } = [];
}