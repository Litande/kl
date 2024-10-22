using KL.Manager.API.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Manager.API.Persistent.EntityConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}
