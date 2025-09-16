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
        Resource res => Uri == res.Uri && LocalName == res.LocalName,
        _ => false
    };
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Uri?.OriginalString.GetHashCode() ?? 0);
            hash = hash * 31 + (LocalName?.GetHashCode() ?? 0);
            return hash;
        }
    }
}