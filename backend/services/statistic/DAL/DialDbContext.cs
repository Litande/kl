using KL.Statistics.Application.Models.Entities;
using KL.Statistics.DAL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;

namespace KL.Statistics.DAL;

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
    public virtual DbSet<CallDetailRecord> CallDetailRecords { get; set; } = null!;
    public virtual DbSet<LeadQueue> LeadQueues { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new CallDetailRecordEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new LeadQueueEntityConfiguration());
        builder.ApplyConfiguration(new UserLeadQueueEntityConfiguration());
    }
}
