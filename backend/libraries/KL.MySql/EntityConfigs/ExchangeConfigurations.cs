using KL.MySql.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.MySql.EntityConfigs;

public class ExchangeConfigurations : IEntityTypeConfiguration<Exchange>
{
    public void Configure(EntityTypeBuilder<Exchange> builder)
    {   
        builder.ToTable("exchange");
        
        builder.Property(i => i.Id).HasColumnName("id");
        builder.Property(i => i.Name).HasColumnName("name");
        builder.Property(i => i.TradingHours).HasColumnName("trading_hours");

        builder.HasKey(i => i.Id);
    }
}