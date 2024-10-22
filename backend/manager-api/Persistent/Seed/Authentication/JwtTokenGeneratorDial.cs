using System.Security.Claims;
using KL.Auth.Services.Identity;
using KL.Manager.API.Application.Common;
using KL.Manager.API.Persistent.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KL.Manager.API.Persistent.Seed.Authentication;

public class JwtTokenGeneratorDial(
    IOptions<JwtConfigurations> jwtConfigurations,
    KlDbContext klDbContext)
    : JwtTokenGenerator<User, long>(jwtConfigurations)
{
    protected override Claim[] GetCustomCaliClaims(User identityUser)
    {
        var user = klDbContext.Users
            .FirstOrDefault(r => r.Id == identityUser.Id);

        var baseClaims = base.GetCustomCaliClaims(identityUser);

        if (user is null) return baseClaims;

        var clientIdClaim = new Claim(CustomClaimTypes.ClientId, user.ClientId.ToString());
        return baseClaims.Append(clientIdClaim).ToArray();
    }
}
