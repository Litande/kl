using KL.Engine.Rule.Enums;
using KL.Engine.Rule.Models;
using KL.Engine.Rule.Repositories;
using KL.Engine.Rule.RuleEngine.Actions;
using RulesEngine.Models;

namespace KL.Engine.Rule.RuleEngine.MicrosoftEngine;

public class MicrosoftRuleEngineProcessingService : IRuleEngineProcessingService
{
    internal const string LeadInputName = "lead";
    internal const string CDRInputName = "cdr";
    internal const string RuleActionProperty = "customActions";

    protected readonly ILogger<MicrosoftRuleEngineProcessingService> _logger;
    protected readonly IEngineMapper _engineMapper;
    protected readonly ICDRRepository _cdrRepository;
    private readonly ISettingsRepository _settingsRepository;

    public MicrosoftRuleEngineProcessingService(
        ILogger<MicrosoftRuleEngineProcessingService> logger,
        IEngineMapper engineMapper,
        ICDRRepository cdrRepository,
        ISettingsRepository settingsRepository
        )
    {
        _logger = logger;
        _engineMapper = engineMapper;
        _cdrRepository = cdrRepository;
        _settingsRepository = settingsRepository;
    }

    public async Task Process(
        long clientId,
        IReadOnlyCollection<TrackedLead> leads,
        IReadOnlyCollection<RuleDto> rules,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        if (!leads.Any()) return;

        if (rules is null || !rules.Any())
        {
            _logger.LogError("Not any rules where found for lead Ids {leadIds}",
                string.Join(", ", leads.Select(r => r.LeadId)));

            throw new ArgumentException("Rules cannot be null or empty");
        }
        var clientEngineMapper = new ClientEngineMapper(clientId, _settingsRepository, _engineMapper, _logger);
        var rulesImpl = await clientEngineMapper.MapToEngineRule(rules, ruleType, RuleActionProperty);

        if (!rulesImpl.Any())
            throw new ArgumentException("Rule mapping failed");

        const string workFlowName = nameof(MicrosoftRuleEngineProcessingService);
        var workFlow = new Workflow
        {
            WorkflowName = workFlowName,
            Rules = rulesImpl,
        };

        var reSettings = new ReSettings
        {
            CustomTypes = new[]
            {
                typeof (LeadStatusTypes),
                typeof (ConditionsHelper),
            }
        };

        var rulesEngine = new RulesEngine.RulesEngine(new[] { workFlow }, reSettings);

        foreach (var lead in leads)
        {
            var inputRuleParam1 = new RuleParameter(LeadInputName, lead);
            var inputRuleParam2 = new RuleParameter(CDRInputName, _cdrRepository);
            var results = await rulesEngine.ExecuteAllRulesAsync(workFlowName, inputRuleParam1, inputRuleParam2);

            await TryExecuteActions(lead, results);
        }
    }

    private async Task TryExecuteActions(TrackedLead lead, List<RuleResultTree> results)
    {
        foreach (var result in results)
        {
            var actions = (List<IRuleActionExecutor>)result.Rule.Properties[RuleActionProperty];
            if (result.IsSuccess)
            {
                foreach (var actionExecutor in actions)
                {
                    try
                    {
                        await actionExecutor.Process(lead);
                    }
                    catch (Exception e)
                    {
                        _logger.LogError(e, "Action execution failed");
                    }
                }
            }
            else if (!string.IsNullOrEmpty(result.ExceptionMessage))
                _logger.LogWarning("Rule failed with exception `{exceptionMessage}`", result.ExceptionMessage);
        }
    }
}
