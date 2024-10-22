using System;

namespace KL.Engine.Rule.Tests.IntegrationTests;

public class LeadProcessingBackgroundServiceTestsFixture
{
    public LeadProcessingBackgroundServiceTestsFixture()
    {
        Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Tests");
        Environment.SetEnvironmentVariable("CLIENTS:SQL:HOST", "localhost");
        Environment.SetEnvironmentVariable("CLIENTS:SQL:PASS", "masterkey");
        Environment.SetEnvironmentVariable("CLIENTS:SQL:USER", "root");
        Environment.SetEnvironmentVariable("CLIENTS:SQL:PORT", "3306");
    }
}
