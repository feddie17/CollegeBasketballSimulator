using Microsoft.AspNetCore.Mvc.Testing;

namespace CBBSimulator.Web.Tests;

public class HealthEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public HealthEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task HealthEndpoint_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/health");

        response.EnsureSuccessStatusCode();
    }
}

public class TeamsEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public TeamsEndpointTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task GetTeams_ReturnsOk()
    {
        var client = _factory.CreateClient();

        var response = await client.GetAsync("/api/teams");

        response.EnsureSuccessStatusCode();
    }
}
