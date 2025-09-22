namespace FSR.DigitalTwin.Domain.SharedKernel.Resource;

public abstract class Resource : IEquatable<Resource>
{
    public abstract Uri? Uri { get; init; }
    public abstract string? LocalName { get; init; }
    public abstract string? Name { get; }
    public virtual StreamReader GetStreamReader() => StreamReader.Null;
    public virtual byte[] GetBytes() => [];
    public virtual int GetContentLength() => 0;
    public override bool Equals(object? other) => Equals(other as Resource);
    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 31 + (Uri?.ToString().GetHashCode() ?? 0);
            hash = hash * 31 + (LocalName?.GetHashCode() ?? 0);
            return hash;
        }
    }
    public bool Equals(Resource? other)
    {
        if (other == null) return false;
        bool result = (Uri?.ToString() == other.Uri?.ToString()) && (LocalName == other.LocalName);
        return result;
    }
}