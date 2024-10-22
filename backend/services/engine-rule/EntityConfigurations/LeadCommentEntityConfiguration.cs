using KL.Engine.Rule.Models.Entities;

namespace KL.Engine.Rule.EntityConfigurations;

public class LeadCommentEntityConfiguration : IEntityTypeConfiguration<LeadComment>
{
    public void Configure(EntityTypeBuilder<LeadComment> builder)
    {
        builder.ToTable("lead_comment");
        builder.Property(e => e.Id).HasColumnName("id");
        builder.Property(e => e.LeadId).HasColumnName("lead_id");
        builder.Property(e => e.Comment).HasColumnName("comment");
        builder.Property(e => e.CreatedAt).HasColumnName("created_at");
        builder.Property(e => e.CreatedById).HasColumnName("created_by");
        builder.HasKey(e => e.Id).HasName("PRIMARY");
    }
}