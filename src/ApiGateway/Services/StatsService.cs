namespace ApiGateway.Services;

public class StatsService
{
    private int _totalRequests = 0;
    private int _blockedRequests = 0;

    public void IncrementTotal() => Interlocked.Increment(ref _totalRequests);
    public void IncrementBlocked() => Interlocked.Increment(ref _blockedRequests);

    public object GetStats() => new
    {
        Total = _totalRequests,
        Blocked = _blockedRequests,
        Allowed = _totalRequests - _blockedRequests
    };
}
