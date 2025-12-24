namespace ApiGateway;

public class TokenBucket
{
    private long _capacity;
    private readonly double _tokensPerSecond;
    private double _currentTokens;
    private DateTime _lastRefillTime;
    private readonly object _lock = new object();

    public TokenBucket(long capacity, double tokensPerSecond)
    {
        _capacity = capacity;
        _tokensPerSecond = tokensPerSecond;
        _currentTokens = capacity;
        _lastRefillTime = DateTime.UtcNow;
    }

    public bool AllowRequest(int tokensNeeded = 1)
    {
        lock (_lock)
        {
            Refill();
            if (_currentTokens < tokensNeeded) return false;

            _currentTokens -= tokensNeeded;
            return true;
        }
    }

    private void Refill()
    {
        var now = DateTime.UtcNow;
        double secondsPassed = (now - _lastRefillTime).TotalSeconds;
        double newTokens = secondsPassed * _tokensPerSecond;

        if (newTokens > 0)
        {
            _currentTokens = Math.Min(_capacity, _currentTokens + newTokens);
            _lastRefillTime = now;
        }
    }
}