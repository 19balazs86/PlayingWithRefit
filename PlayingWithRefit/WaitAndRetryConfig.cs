namespace PlayingWithRefit;

public sealed class WaitAndRetryConfig
{
    public int Retry { get; init; }
    public int Wait { get; init; }
    public int Timeout { get; init; }
}
