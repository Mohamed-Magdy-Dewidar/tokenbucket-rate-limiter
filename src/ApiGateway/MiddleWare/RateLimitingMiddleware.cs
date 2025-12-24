using ApiGateway;
using ApiGateway.Services; 
using System.Collections.Concurrent;
using System.Net;

public class RateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly StatsService _stats; 
    private static readonly ConcurrentDictionary<string, TokenBucket> _buckets = new();

    private const int CAPACITY = 10;
    private const double REFILL_RATE = 1.0;

    public RateLimitingMiddleware(RequestDelegate next, StatsService stats)
    {
        _next = next;
        _stats = stats;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. IGNORE the stats endpoint (Dashboard)
        if (context.Request.Path.StartsWithSegments("/api/stats"))
        {
            await _next(context);
            return;
        }

        _stats.IncrementTotal();

        // 2. EXTRACT the API Key from Headers
        // If the header is missing, it defaults to null (Anonymous User)
        string? apiKey = context.Request.Headers["X-Api-Key"].FirstOrDefault();

        // 3. DEFINE your Security Policies
        // Default Policy (Free Tier / Anonymous)
        int capacity = 10;
        double refillRate = 1.0;
        string tierName = "Free";

        // VIP Policy (Gold Tier)
        // In a real app, you would check a database here.
        // For the demo, we check against a hardcoded "Golden Key".
        if (apiKey == "GOLD-USER-777")
        {
            capacity = 50;       // 5x more bursting power
            refillRate = 5.0;    // 5x faster refill
            tierName = "Gold";
        }

        // 4. GET or CREATE the Bucket
        // We use the API Key as the unique ID. If no key, we fall back to IP address.
        string userIdentifier = apiKey ?? context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

        var bucket = _buckets.GetOrAdd(userIdentifier, _ => new TokenBucket(capacity, refillRate));

        // 5. CHECK LOGIC
        if (!bucket.AllowRequest())
        {
            _stats.IncrementBlocked();
            context.Response.StatusCode = 429;

            // (Optional) Return a helpful error message
            await context.Response.WriteAsJsonAsync(new
            {
                Error = "Rate limit exceeded",
                CurrentTier = tierName,
                UpgradeHint = "Contact sales to upgrade to Gold Tier."
            });
            return;
        }

        await _next(context);
    }
}