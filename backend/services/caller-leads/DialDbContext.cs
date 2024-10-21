using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadCaller.Application.Models.Entities;
using Plat4Me.DialLeadCaller.Infrastructure.EntityConfigurations;

namespace Plat4Me.DialLeadCaller.Infrastructure;

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
    public virtual DbSet<LeadHistory> LeadHistory { get; set; } = null!;
    public virtual DbSet<LeadQueue> LeadQueues { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<UserLeadQueue> UserLeadQueue { get; set; } = null!;
    public virtual DbSet<CallDetailRecord> CallDetailRecords { get; set; } = null!;
    public virtual DbSet<SettingsEntity> Settings { get; set; } = null!;
    public virtual DbSet<SipProvider> SipProviders { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new LeadEntityConfiguration());
        builder.ApplyConfiguration(new LeadHistoryEntityConfiguration());
        builder.ApplyConfiguration(new LeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new UserLeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new CallDetailRecordEntityConfiguration());
        builder.ApplyConfiguration(new DataSourceEntityConfiguration());
        builder.ApplyConfiguration(new TagEntityConfiguration());
        builder.ApplyConfiguration(new UserTagEntityConfiguration());
        builder.ApplyConfiguration(new SettingsEntityConfiguration());
        builder.ApplyConfiguration(new SipProviderEntityConfiguration());
    }
}
