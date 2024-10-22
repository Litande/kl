using KL.MySql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.MySql.EntityConfigs;

public class InstrumentConfigurations : IEntityTypeConfiguration<Instrument>
{
    public void Configure(EntityTypeBuilder<Instrument> builder)
    {   
        builder.ToTable("instrument");
        
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.Name).HasColumnName("name");
        builder.Property(i => i.ProviderSymbol).HasColumnName("provider_symbol");
        builder.Property(i => i.UnixSymbol).HasColumnName("unix_symbol");
        builder.Property(i => i.Status).HasColumnName("status");
        builder.Property(i => i.FeedId).HasColumnName("feed_id");
        builder.Property(i => i.CategoryId).HasColumnName("category_id");
        builder.Property(i => i.ExchangeId).HasColumnName("exchange_id");
        builder.Property(i => i.QuotePrecision).HasColumnName("quote_precision");
        builder.Property(i => i.TickTimeout).HasColumnName("tick_timeout");

        builder.HasKey(i => i.Id);
    }
}