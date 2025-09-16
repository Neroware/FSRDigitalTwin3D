namespace FSR.DigitalTwin.Domain.Model.Process.HRC.Task;

public record FunctionPropertyData
{
    public required Resource Function { init; get; }
    public string? ProcedureName { init; get; }
    public string? ProcedureDescription { init; get; }
    public string? ProcedureId { init; get; }
    public long Duration { init; get; }
    public long DurationUncertainty { init; get; }
}

public record FunctionObjectData
{
    public required Resource Function { init; get; }
    public HashSet<Resource> Target { init; get; } = [];
    public HashSet<Resource> StartLocation { init; get; } = [];
    public HashSet<Resource> EndLocation { init; get; } = [];
    public HashSet<Resource> Location { init; get; } = [];
}