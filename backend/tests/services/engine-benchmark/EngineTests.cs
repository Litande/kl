using BenchmarkDotNet.Attributes;

namespace KL.Engine.Benchmark.Tests;

public class EngineTests : LeadProcessingPipelineServiceTestsFixture
{
    private readonly MicrosoftRuleEngineProcessingService _service;
    private readonly List<LeadDto> _leads = new();
    private readonly List<RuleDto> _queueRules = new();
    private readonly List<RuleDto> _scoreRules = new();

    private readonly Dictionary<RulesCondition, IRuleCondition> _conditions = new();
    private readonly Dictionary<RulesAction, IRuleAction> _actions = new();

    public EngineTests()
    {
        _service = new MicrosoftRuleEngineProcessingService();
        var rand = new Random();
        for (var i = 0; i < 10000; i++)
        {
            _leads.Add(new LeadDto
            {
                FirstName = "First_" + i,
                LastName = "Last_" + i,
                Score = rand.Next(100, 10000),
                Status = (LeadStatusTypes)rand.Next(1, 29),
                RegistrationTime = DateTimeOffset.UtcNow.AddMinutes(-rand.Next(1, 150)),
            });
        }

        _conditions.Add(RulesCondition.PreviousStatus, new PreviousStatus());
        _actions.Add(RulesAction.ChangeStatus, new ChangeStatus());

        //_queueRules.AddRange(GetQueueFakeRules70PrcEasyAnd30PrcExt(100));
        //_scoreRules.AddRange(GetScoreFakeRules70PrcEasyAnd30PrcExt(100));
        _queueRules.AddRange(GetQueueFakeRulesPreviousStatusAndChangeStatus());
    }

    [Benchmark]
    public async Task QueueProcess10kLeadsAnd()
    {
        var results = await _service.Process(
           _leads,
           _queueRules,
           _conditions,
           _actions
           );
    }


    // [Benchmark]
    // public async Task QueueProcess50kLeadsAnd()
    // {
    //     //var results = await _service.Process(
    //     //    _leads,
    //     //    _queueRules,
    //     //    new RuleActionExecutor<LeadDto>(RuleActionTypes.Replace, nameof(LeadDto.LeadQueueId)));
    //     //(item, value) => item.LeadQueueId = value);
    // }

    // [Benchmark]
    // public async Task ScoreProcess50kLeadsAnd()
    // {
    //     //var results = await _service.Process(
    //     //    _leads,
    //     //    _scoreRules,
    //     //    new RuleActionExecutor<LeadDto>(RuleActionTypes.Increase, nameof(LeadDto.Score)));
    //     //(item, value) => item.Score += value);
    // }
}
