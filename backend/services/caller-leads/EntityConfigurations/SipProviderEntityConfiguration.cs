using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadCaller.Application.Enums;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Infrastructure.EntityConfigurations;

public class SipProviderEntityConfiguration : IEntityTypeConfiguration<SipProvider>
{
    public void Configure(EntityTypeBuilder<SipProvider> builder)
    {
        builder.ToTable("sip_provider");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.ProviderName).HasColumnName("provider_name");
        builder.Property(e => e.ProviderAddress).HasColumnName("provider_address");
        builder.Property(e => e.ProviderUserName).HasColumnName("provider_username");
        builder.Property(e => e.Status).HasColumnName("status")
            .HasConversion(
                r => r.ToString().ToLowerInvariant(),
                r => Enum.Parse<SipProviderStatus>(r, true));
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}