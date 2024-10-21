using Microsoft.EntityFrameworkCore;
using Plat4Me.DialLeadProvider.Persistent.Entities;
using Plat4Me.DialLeadProvider.Persistent.EntityConfigurations;
using TimeZone = Plat4Me.DialLeadProvider.Persistent.Entities.TimeZone;

namespace Plat4Me.DialLeadProvider.Persistent;

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
    public virtual DbSet<UserDataSourceMap> UserDataSourceMaps { get; set; } = null!;
    public virtual DbSet<LeadDataSourceMap> LeadDataSourceMaps { get; set; } = null!;
    public virtual DbSet<StatusDataSourceMap> StatusDataSourceMaps { get; set; } = null!;
    public virtual DbSet<DataSource> DataSources { get; set; } = null!;
    public virtual DbSet<User> Users { get; set; } = null!;
    public virtual DbSet<SettingsEntity> Settings { get; set; } = null!;
    public virtual DbSet<TimeZone> TimeZones { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new LeadEntityConfiguration());
        builder.ApplyConfiguration(new LeadHistoryEntityConfiguration());
        builder.ApplyConfiguration(new CountryEntityConfiguration());
        builder.ApplyConfiguration(new LanguageEntityConfiguration());
        builder.ApplyConfiguration(new LeadDataSourceMapEntityConfiguration());
        builder.ApplyConfiguration(new StatusDataSourceMapEntityConfiguration());
        builder.ApplyConfiguration(new DataSourceEntityConfiguration());
        builder.ApplyConfiguration(new UserDataSourceMapEntityConfiguration());
        builder.ApplyConfiguration(new UserEntityConfiguration());
        builder.ApplyConfiguration(new ClientEntityConfiguration());
        builder.ApplyConfiguration(new SettingsEntityConfiguration());
        builder.ApplyConfiguration(new TimeZoneEntityConfiguration());
    }
}