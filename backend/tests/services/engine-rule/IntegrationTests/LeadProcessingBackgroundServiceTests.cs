using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Plat4Me.DialRuleEngine.Application.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Plat4Me.DialRuleEngine.Tests.IntegrationTests;

public class LeadProcessingBackgroundServiceTests : LeadProcessingBackgroundServiceTestsFixture
{
    [Theory]
    //[InlineData(1)]
    [InlineData(15, new long[] { 1, 2, 3 })]
    [InlineData(99)]
    public async Task GetLeadsQueue_ShouldReturnAll_AfterRequestGetNext(long queueId, IEnumerable<long>? agentIds = null)
    {
        // var application = new WebApplicationFactory<Program>();
        // var client = application.CreateClient();

        // await Task.Delay(1000);

        // var jsonString = JsonSerializer.Serialize(agentIds);
        // HttpContent httpContent = new StringContent(jsonString);
        // httpContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/json");

        // var response1 = await client.PutAsync($"/lead-queue/{queueId}/get-next", httpContent);
        // Assert.True(response1.IsSuccessStatusCode);
        // var body1 = await response1.Content.ReadAsStringAsync();
        // var result1 = JsonSerializer.Deserialize<LeadDto>(body1, JsonSettingsExtensions.Default);
        // Assert.True(result1.LeadQueueId == queueId);

        // var response2 = await client.GetAsync($"/lead-queue/{queueId}");
        // Assert.True(response2.IsSuccessStatusCode);
        // var body2 = await response2.Content.ReadAsStringAsync();
        // var result2 = JsonSerializer.Deserialize<LeadQueueUpdated>(body2, JsonSettingsExtensions.Default);
        // result2.Items.Should().HaveCount(1);
    }
}
