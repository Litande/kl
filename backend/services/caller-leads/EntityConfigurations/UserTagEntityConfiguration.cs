using KL.Caller.Leads.Models.Entities;

namespace KL.Caller.Leads.EntityConfigurations;

public class UserTagEntityConfiguration : IEntityTypeConfiguration<UserTag>
{
    public void Configure(EntityTypeBuilder<UserTag> builder)
    {
        builder.ToTable("user_tag");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.TagId).HasColumnName("tag_id");
        builder.Property(e => e.ExpiredOn).HasColumnName("expired_on");
        builder.HasKey(e => new { e.UserId, e.TagId }).HasName("PRIMARY");
    }
}