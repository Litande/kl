using KL.Provider.Leads.Persistent.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace KL.Provider.Leads.Persistent.EntityConfigurations;

public class UserDataSourceMapEntityConfiguration : IEntityTypeConfiguration<UserDataSourceMap>
{
    public void Configure(EntityTypeBuilder<UserDataSourceMap> builder)
    {
        builder.ToTable("user_data_source_map");
        builder.Property(e => e.UserId).HasColumnName("user_id");
        builder.Property(e => e.DataSourceId).HasColumnName("data_source_id");
        builder.Property(e => e.EmployeeId).HasColumnName("employee_id");
        builder.HasKey(e => e.UserId).HasName("PRIMARY");
    }
}