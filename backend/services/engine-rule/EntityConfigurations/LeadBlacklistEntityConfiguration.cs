using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.EntityConfigurations;

public class LeadBlacklistEntityConfiguration : IEntityTypeConfiguration<LeadBlacklist>
{
    public void Configure(EntityTypeBuilder<LeadBlacklist> builder)
    {
        builder.ToTable("lead_blacklist");
        builder.Property(e => e.Id).HasColumnName("id");
        // builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.LeadId).HasColumnName("lead_id");
        builder.Property(e => e.CreatedByUserId).HasColumnName("created_by");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}