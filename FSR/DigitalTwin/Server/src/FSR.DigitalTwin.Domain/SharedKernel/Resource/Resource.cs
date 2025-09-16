namespace FSR.DigitalTwin.Domain.SharedKernel.Resource;

public abstract class Resource
{
    public abstract Uri? Uri { get; init; }
    public abstract string? LocalName { get; init; }
    public abstract string? Name { get; }
    public virtual StreamReader GetStreamReader() => StreamReader.Null;
    public virtual byte[] GetBytes() => [];
    public virtual int GetContentLength() => 0;
    public override bool Equals(object? other) => other switch
    {
        null => false,
        Resource => Uri != null && ((Resource)other).Uri == Uri,
        _ => false
    };
    public override int GetHashCode() => Uri?.GetHashCode() ?? 0;
}