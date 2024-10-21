using Microsoft.Extensions.Options;
using Moq;
using Plat4Me.DialLeadCaller.Application.Models.Requests;
using Plat4Me.DialLeadCaller.Infrastructure.App;
using Plat4Me.DialLeadCaller.Infrastructure.Clients;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Plat4Me.DialLeadCaller.Tests;

public class LeadRuleEngineClientTests
{
    [Fact]
    public async Task RunApplication_ShouldReturnOk_WhenApplicationStarts()
    {
        var httpClientFactoryMock = new Mock<IHttpClientFactory>();
        var leadQueueClientOptionsMock = new Mock<IOptions<LeadQueueClientOptions>>();

        var httpClient = new HttpClient();
        httpClientFactoryMock
            .Setup(r => r.CreateClient(nameof(LeadRuleEngineClient)))
            .Returns(httpClient);

        var leadQueueClientOptions = new LeadQueueClientOptions
        {
            BaseUrl = "https://localhost:7151/",
            GetNextEndpoint = "lead-queue/{queueId}/get-next",
            RunBehaviourRulesEndpoint = ""
        };
        leadQueueClientOptionsMock
            .Setup(r => r.Value)
            .Returns(leadQueueClientOptions);

        var client = new LeadRuleEngineClient(httpClientFactoryMock.Object, leadQueueClientOptionsMock.Object);

        var request = new GetNextLeadRequest(1, new long[]{ 1 });
        var response = await client.GetNextLead(clientId: 1, request);

        Assert.Null(response);
    }
}