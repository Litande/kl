using KL.Agent.API.Persistent.Entities;
using KL.Agent.API.Persistent.EntityConfigurations;
using KL.Auth.Persistence;
using Microsoft.EntityFrameworkCore;

namespace KL.Agent.API.Persistent;

public class KlDbContext(DbContextOptions<KlDbContext> options) : KlAuthDbContext<User, Role, long>(options)
{
    public virtual DbSet<Client> Clients { get; set; } = null!;
    public virtual DbSet<Lead> Leads { get; set; } = null!;
    public virtual DbSet<LeadHistory> LeadHistory { get; set; } = null!;
    public virtual DbSet<StatusRule> StatusRules { get; set; } = null!;
    public virtual DbSet<CallDetailRecord> CallDetailRecords { get; set; } = null!;
    public virtual DbSet<SettingsEntity> Settings { get; set; } = null!;
    public virtual DbSet<LeadComment> LeadComments { get; set; } = null!;
    public virtual DbSet<AgentStatusHistory> AgentStatusHistories { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new RoleEntityConfiguration());
        builder.ApplyConfiguration(new LeadEntityConfiguration());
        builder.ApplyConfiguration(new LeadHistoryEntityConfiguration());
        builder.ApplyConfiguration(new DataSourceEntityConfiguration());
        builder.ApplyConfiguration(new StatusRuleEntityConfiguration());
        builder.ApplyConfiguration(new CallDetailRecordEntityConfiguration());
        builder.ApplyConfiguration(new SettingsEntityConfiguration());
        builder.ApplyConfiguration(new LeadCommentEntityConfiguration());
        builder.ApplyConfiguration(new AgentStatusHistoryEntityConfiguration());
    }
}
