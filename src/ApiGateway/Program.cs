
using ApiGateway.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddHttpClient();
builder.Services.AddControllers(); // Add this

// 1. Register the Stats Service
builder.Services.AddSingleton<StatsService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

// 2. Enable Static Files (Serves index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

// 3. Register Middleware
app.UseMiddleware<RateLimitingMiddleware>();

app.MapControllers(); // Maps the StatsController

app.UseHttpsRedirection();



// 2. Define the Proxy Route
app.MapGet("/api/content/{id}", async (int id, IHttpClientFactory httpClientFactory) =>
{
    var client = httpClientFactory.CreateClient();

    try
    {    
        var response = await client.GetAsync($"http://localhost:5001/api/content/{id}");

        // 2. Return whatever the CoreService gave us
        var content = await response.Content.ReadAsStringAsync();
        return Results.Content(content, response.Content.Headers.ContentType?.ToString() ?? "application/json");
    }
    catch (Exception ex)
    {
        return Results.Problem($"Gateway Error: {ex.Message}");
    }
});


app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
