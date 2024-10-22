using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace KL.Auth.Persistence;

public class KLAuthDbContext<TUser, TRole, TId>(DbContextOptions<KLAuthDbContext<TUser, TRole, TId>> options)
    :
        IdentityDbContext<TUser, TRole, TId,
            IdentityUserClaim<TId>,
            IdentityUserRole<TId>,
            IdentityUserLogin<TId>,
            IdentityRoleClaim<TId>,
            IdentityUserToken<TId>
        >(options)
    where TUser : IdentityUser<TId>
    where TRole : IdentityRole<TId>
    where TId : IEquatable<TId>
{
}