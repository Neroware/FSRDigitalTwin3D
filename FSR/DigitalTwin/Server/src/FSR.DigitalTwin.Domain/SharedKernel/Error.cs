namespace FSR.DigitalTwin.Domain.SharedKernel;

public record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "Null value was provided");
    public static readonly Error DatabaseNotConnectedFailure = new("Error.DatabaseConnectionFailure", "A database connections was expected for the task, but no connection was established");
    public static readonly Error MaximalRetriesExhausted = new("Error.MaximalRetriesExhausted", "When trying to execute the command the maximum retries were exhausted without success.");

    public Error(Exception ex) : this(Code: ex.GetType().ToString(), Description: ex.Message) { }

    public static implicit operator Result(Error error) => Result.Failure(error);

    public Result ToResult() => Result.Failure(this);
}
