using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Plat4Me.Dial.Statistic.Api.Application.Models.Entities;

namespace Plat4Me.Dial.Statistic.Api.DAL.EntityConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
