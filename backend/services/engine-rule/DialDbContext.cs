using Microsoft.EntityFrameworkCore;
using Plat4Me.DialRuleEngine.Application.Models.Entities;
using Plat4Me.DialRuleEngine.Infrastructure.EntityConfigurations;

namespace Plat4Me.DialRuleEngine.Infrastructure;

public class DialDbContext : DbContext
{
    public DialDbContext()
    {
    }

    public DialDbContext(DbContextOptions<DialDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; } = null!;
    public virtual DbSet<Lead> Leads { get; set; } = null!;
    public virtual DbSet<LeadQueue> LeadQueues { get; set; } = null!;
    public virtual DbSet<LeadHistory> LeadHistory { get; set; } = null!;
    public virtual DbSet<Rule> Rules { get; set; } = null!;
   public virtual DbSet<StatusDataSourceMap> StatusDataSources { get; set; } = null!;
    public virtual DbSet<DataSource> DataSources { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<RuleGroup> RuleGroups { get; set; } = null!;
    public virtual DbSet<CallDetailRecord> CallDetailRecords { get; set; } = null!;
    public virtual DbSet<SettingsEntity> Settings { get; set; } = null!;
    public virtual DbSet<LeadComment> LeadComments { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new LeadEntityConfiguration());
        builder.ApplyConfiguration(new LeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new RuleEntityConfiguration());
        builder.ApplyConfiguration(new StatusDataSourceMapEntityConfiguration());
        builder.ApplyConfiguration(new DataSourceEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new LeadHistoryEntityConfiguration());
        builder.ApplyConfiguration(new RuleGroupEntityConfiguration());
        builder.ApplyConfiguration(new LeadBlacklistEntityConfiguration());
        builder.ApplyConfiguration(new CallDetailRecordEntityConfiguration());
        builder.ApplyConfiguration(new SettingsEntityConfiguration());
        builder.ApplyConfiguration(new LeadCommentEntityConfiguration());
    }
}
