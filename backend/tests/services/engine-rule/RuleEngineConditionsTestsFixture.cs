using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace KL.Engine.Rule.Tests;

public class RuleEngineConditionsTestsFixture : TestBase
{
    protected readonly Mock<ILogger<RuleEngineMock>> RuleEngineLoggerMock = new();
    protected readonly Mock<ILogger<MicrosoftEngineMapper>> RuleMapperLoggerMock = new();
    protected readonly Mock<ICDRRepository> CdrRepositoryMock = new();
    protected readonly Mock<ISettingsRepository> SettingsRepositoryMock = new();

    // Services
    private DialDbContext _context;
    private ICDRRepository _cdrRepository;

    // Fills
    protected void Fill_CallDetailRecords(IEnumerable<TrackedLead>? leads, int count = 3)
    {
        // cleanup first
        _context.CallDetailRecords.RemoveRange(_context.CallDetailRecords.ToArray());

        var leadIds = leads is null
            ? new long[] { 1 }
            : leads.Select(r => r.LeadId);

        foreach (var leadId in leadIds)
        {
            for (var i = 0; i < count; i++)
            {
                var id = long.Parse($"{leadId}{i}");
                _context.CallDetailRecords.Add(new CallDetailRecord
                {
                    Id = id,
                    LeadId = leadId,
                    OriginatedAt = DateTimeOffset.UtcNow.AddDays(-i).AddMinutes(-(i + 4)),
                    LeadAnswerAt = DateTimeOffset.UtcNow.AddDays(-i).AddMinutes(-(i + 3)),
                    UserAnswerAt = DateTimeOffset.UtcNow.AddDays(-i).AddMinutes(-(i + 2)),
                    CallHangupAt = DateTimeOffset.UtcNow.AddDays(-i).AddMinutes(-(i + 1)),
                    CallHangupStatus = CallFinishReasons.CallFinishedByAgent,
                    CallType = CallType.Predictive,
                    SessionId = Guid.NewGuid().ToString(),
                    LeadPhone = "test-lead-phone-" + id,
                    LeadStatusAfter = null,
                    LeadStatusBefore = LeadStatusTypes.NewLead,
                    LastUserId = id,
                    IsReplacedUser = false,
                    CallDuration = 100,
                });
            }
        }

        _context.SaveChanges();
    }

    protected void Clear_CDR()
    {
        _context.CallDetailRecords.RemoveRange(_context.CallDetailRecords.ToArray());
    }

    protected void Create_CallDetailRecords(params CallDetailRecord[] cdrs)
    {
        _context.CallDetailRecords.AddRange(cdrs);
        _context.SaveChanges();
    }

    protected void PrepareCdrRepository(bool useInMemoryDb = false)
    {
        if (useInMemoryDb)
        {
            var dbContextOptions = new DbContextOptionsBuilder<DialDbContext>()
                .UseInMemoryDatabase(databaseName: "test-kl-db-context-in-memory-1")
                .Options;

            _context = new DialDbContext(dbContextOptions);
            _cdrRepository = new CDRRepository(_context);
        }
        else
        {
            _cdrRepository = CdrRepositoryMock.Object;
        }
    }

    protected RuleEngineMock GetRuleProcessingService()
    {
        return new RuleEngineMock(
            RuleEngineLoggerMock.Object,
            new MicrosoftEngineMapper(RuleMapperLoggerMock.Object),
            _cdrRepository,
            SettingsRepositoryMock.Object
        );
    }
}

public class RuleEngineMock : MicrosoftRuleEngineProcessingService
{
    public RuleEngineMock(
        ILogger<RuleEngineMock> logger,
        IEngineMapper engineMapper,
        ICDRRepository cdrRepository,
        ISettingsRepository repository) :
        base(logger, engineMapper, cdrRepository, repository)
    {
    }

    public async Task<Dictionary<long, List<RuleResultTree>>?> ProcessConditions(
        IReadOnlyCollection<TrackedLead> leads,
        IReadOnlyCollection<RuleDto> rules,
        RuleGroupTypes ruleType,
        CancellationToken ct = default)
    {
        if (!leads.Any()) return null;

        if (rules is null || !rules.Any())
        {
            _logger.LogError("Not any rules where found for lead Ids {LeadIds}",
                string.Join(", ", leads.Select(r => r.LeadId)));

            throw new ArgumentException("Rules cannot be null or empty");
        }

        var rulesImpl = await _engineMapper.MapToEngineRule(rules, ruleType, RuleActionProperty);

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
                typeof(LeadStatusTypes),
                typeof(ConditionsHelper),
            }
        };

        var rulesEngine = new RulesEngine.RulesEngine(new[] { workFlow }, reSettings);

        var list = new Dictionary<long, List<RuleResultTree>>();
        foreach (var lead in leads)
        {
            var inputRuleParam1 = new RuleParameter(LeadInputName, lead);
            var inputRuleParam2 = new RuleParameter(CDRInputName, _cdrRepository);
            var results = await rulesEngine.ExecuteAllRulesAsync(workFlowName, inputRuleParam1, inputRuleParam2);
            list.Add(lead.LeadId, results);
        }

        return list;
    }
}