namespace PlayingWithRefit;

public sealed class WaitAndRetryConfig
{
    public int MaxRetryAttempts { get; init; }
    public int Delay { get; init; }
    public int AttemptTimeout { get; init; }
    public int TotalRequestTimeout { get; init; }
}
