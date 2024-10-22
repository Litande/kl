using System.Security.Claims;
using KL.Auth.Services.Identity;
using KL.Statistics.Application.Common;
using KL.Statistics.DAL;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KL.Statistics.Authentication;

public class JwtTokenGeneratorDial(
    IOptions<JwtConfigurations> jwtConfigurations,
    IDbContextFactory<KlDbContext> dbContextFactory)
    : JwtTokenGenerator<IdentityUser<long>, long>(jwtConfigurations)
{
    protected override Claim[] GetCustomCaliClaims(IdentityUser<long> identityUser)
    {
        using var context = dbContextFactory.CreateDbContext();
        var user = context.Users
            .FirstOrDefault(r => r.Id == identityUser.Id);

        var baseClaims = base.GetCustomCaliClaims(identityUser);

        if (user is null) return baseClaims;

        var clientIdClaim = new Claim(CustomClaimTypes.ClientId, user.ClientId.ToString());
        return baseClaims.Append(clientIdClaim).ToArray();
    }
}
