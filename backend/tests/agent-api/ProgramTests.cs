using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Threading.Tasks;
using Xunit;

namespace Plat4Me.DialAgentApi.Tests;

public class ProgramTests
{
    [Fact]
    public async Task RunApplication_ShouldReturnOk_WhenApplicationStarts()
    {
        await using var application = new WebApplicationFactory<Program>();

        var client = application.CreateClient();
        var healthCheckResponse = await client.GetAsync("/metrics");

        healthCheckResponse.Should().NotBeNull();
        healthCheckResponse.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}