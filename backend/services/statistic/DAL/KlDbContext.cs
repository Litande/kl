using KL.Auth.Persistence;
using KL.Statistics.Application.Models.Entities;
using KL.Statistics.DAL.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Role = KL.Statistics.Application.Models.Entities.Role;

namespace KL.Statistics.DAL;

public class KlDbContext(DbContextOptions<KlDbContext> options) : KlAuthDbContext<User, Role, long>(options)
{

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
