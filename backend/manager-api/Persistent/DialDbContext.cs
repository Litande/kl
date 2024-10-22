using KL.Manager.API.Persistent.Entities;
using KL.Manager.API.Persistent.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace KL.Manager.API.Persistent;

public class DialDbContext : DbContext
{
    public DialDbContext(DbContextOptions<DialDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Client> Clients { get; set; } = null!;
    public virtual DbSet<Lead> Leads { get; set; } = null!;
    public virtual DbSet<DataSource> DataSources { get; set; } = null!;
    public virtual DbSet<LeadHistory> LeadHistory { get; set; } = null!;
    public virtual DbSet<LeadQueue> LeadQueues { get; set; } = null!;
    public virtual DbSet<Rule> Rules { get; set; } = null!;
    public virtual DbSet<RuleGroup> GroupRules { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<SettingsEntity> Settings { get; set; } = null!;
    public virtual DbSet<StatusRule> StatusRules { get; set; } = null!;
    public virtual DbSet<Team> Teams { get; set; } = null!;
    public virtual DbSet<Tag> Tags { get; set; } = null!;
    public virtual DbSet<CallDetailRecord> CallDetailRecords { get; set; } = null!;
    public virtual DbSet<LeadBlacklist> LeadBlacklists { get; set; } = null!;
    public virtual DbSet<UserFilterPreferences> UserFilterPreferences { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new LeadEntityConfiguration());
        builder.ApplyConfiguration(new LeadHistoryEntityConfiguration());
        builder.ApplyConfiguration(new RuleEntityConfiguration());
        builder.ApplyConfiguration(new RuleGroupEntityConfiguration());
        builder.ApplyConfiguration(new LeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new UserLeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new TeamEntityConfiguration());
        builder.ApplyConfiguration(new UserTeamEntityConfiguration());
        builder.ApplyConfiguration(new SettingsEntityConfiguration());
        builder.ApplyConfiguration(new StatusRuleEntityConfiguration());
        builder.ApplyConfiguration(new TagEntityConfiguration());
        builder.ApplyConfiguration(new UserTagEntityConfiguration());
        builder.ApplyConfiguration(new DataSourceEntityConfiguration());
        builder.ApplyConfiguration(new CallDetailRecordEntityConfiguration());
        builder.ApplyConfiguration(new LeadBlacklistEntityConfiguration());
        builder.ApplyConfiguration(new UserFilterPreferencesEntityConfiguration());
    }
}