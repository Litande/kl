using KL.MySql.Entities;
using KL.MySql.EntityConfigs;
using Microsoft.EntityFrameworkCore;

namespace KL.MySql;

public class AssetDbContext: DbContext
{
    public AssetDbContext(DbContextOptions<AssetDbContext> options)
        : base(options)
    {
    }

    public DbSet<Feed> Feeds { get; set; }
    public DbSet<Exchange> Exchanges { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Instrument> Instruments { get; set; }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new InstrumentConfigurations());
        builder.ApplyConfiguration(new ExchangeConfigurations());
        builder.ApplyConfiguration(new FeedConfigurations());
        builder.ApplyConfiguration(new CategoryConfigurations());
    }
}