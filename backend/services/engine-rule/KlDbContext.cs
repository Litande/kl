using KL.Auth.Persistence;
using KL.Engine.Rule.EntityConfigurations;
using KL.Engine.Rule.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace KL.Engine.Rule;

public class KlDbContext(DbContextOptions<KlDbContext> options) : KlAuthDbContext<User, Role, long>(options)
{

    public virtual DbSet<Client> Clients { get; set; } = null!;
    public virtual DbSet<Lead> Leads { get; set; } = null!;
    public virtual DbSet<LeadQueue> LeadQueues { get; set; } = null!;
    public virtual DbSet<LeadHistory> LeadHistory { get; set; } = null!;
    public virtual DbSet<Models.Entities.Rule> Rules { get; set; } = null!;
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
