using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.DialLeadCaller.Application.Models.Entities;

namespace Plat4Me.DialLeadCaller.Infrastructure.EntityConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}