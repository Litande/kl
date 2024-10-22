using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.EntityConfigurations;

public class ClientEntityConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("client");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}