using System;
using KL.MySql.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KL.Auth.Persistence;

public class KlAuthDbContext<TUser, TRole, TId>
    :
        IdentityDbContext<TUser, TRole, TId,
            IdentityUserClaim<TId>,
            IdentityUserRole<TId>,
            IdentityUserLogin<TId>,
            IdentityRoleClaim<TId>,
            IdentityUserToken<TId>
        >
    where TUser : IdentityUser<TId>
    where TRole : IdentityRole<TId>
    where TId : IEquatable<TId>
{
    public KlAuthDbContext()
    {
        
    }
    
    public KlAuthDbContext(DbContextOptions options) 
        : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        
        builder.Entity<TUser>().ToTable("user");
        builder.Entity<TRole>().ToTable("role");
        builder.Entity<IdentityUserRole<TId>>().ToTable("user_role");
        builder.Entity<IdentityUserClaim<TId>>().ToTable("user_claim");
        builder.Entity<IdentityUserLogin<TId>>().ToTable("user_login");
        builder.Entity<IdentityRoleClaim<TId>>().ToTable("role_claim");
        builder.Entity<IdentityUserToken<TId>>().ToTable("user_token");
        
        foreach(var entity in builder.Model.GetEntityTypes())
        {
            // Replace table names
            entity.SetTableName(entity.GetTableName().ToSnakeCase());

            // Replace column names            
            foreach(var property in entity.GetProperties())
            {
                property.SetColumnName(property.Name.ToSnakeCase());
            }

            foreach(var key in entity.GetKeys())
            {
                key.SetName(key.GetName().ToSnakeCase());
            }

            foreach(var key in entity.GetForeignKeys())
            {
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());
            }

            foreach(var index in entity.GetIndexes())
            {
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
            }
        }
    }
}