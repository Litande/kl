using System;
using KL.Auth.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KL.Auth.Persistence;

public class AuthenticationDbContext<TUser, TRole, TId> :
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
    public AuthenticationDbContext(DbContextOptions<AuthenticationDbContext<TUser, TRole, TId>> options) : base(options)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

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