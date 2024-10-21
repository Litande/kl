using Microsoft.Extensions.Logging;
using Moq;
using Plat4Me.DialRuleEngine.Application.Repositories;
using Plat4Me.DialRuleEngine.Application.RuleEngine;
using Plat4Me.DialRuleEngine.Application.RuleEngine.MicrosoftEngine;

namespace Plat4Me.DialRuleEngine.Tests;

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