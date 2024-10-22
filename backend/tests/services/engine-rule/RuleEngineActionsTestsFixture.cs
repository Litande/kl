using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine;
using KL.Engine.Rule.RuleEngine.MicrosoftEngine;
using Microsoft.Extensions.Logging;
using Moq;

namespace KL.Engine.Rule.Tests;

public class RuleEngineActionsTestsFixture: TestBase
{
    protected readonly Mock<ILogger<MicrosoftRuleEngineProcessingService>> RuleEngineLoggerMock = new();
    protected readonly Mock<ILogger<MicrosoftEngineMapper>> RuleMapperLoggerMock = new();
    protected readonly Mock<ICDRRepository> CdrRepositoryMock = new();
    protected readonly Mock<ISettingsRepository> SettingsRepositoryMock = new();

    // Services
    protected IRuleEngineProcessingService GetRuleProcessingService() =>
        new MicrosoftRuleEngineProcessingService(
            RuleEngineLoggerMock.Object,
            new MicrosoftEngineMapper(RuleMapperLoggerMock.Object),
            CdrRepositoryMock.Object, SettingsRepositoryMock.Object);
}