using System.Security.Claims;
using KL.Agent.API.Application.Common;
using KL.Agent.API.Persistent.Entities;
using KL.Auth.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KL.Agent.API.Persistent.Seed.Authentication;

public class JwtTokenGeneratorDial(
    IOptions<JwtConfigurations> jwtConfigurations,
    KlDbContext dbContext)
    : JwtTokenGenerator<User, long>(jwtConfigurations)
{
    protected override Claim[] GetCustomCaliClaims(User identityUser)
    {
        var user = dbContext.Users
            .FirstOrDefault(r => r.Id == identityUser.Id);

        var baseClaims = base.GetCustomCaliClaims(identityUser);

        if (user is null) return baseClaims;

        var clientIdClaim = new Claim(CustomClaimTypes.ClientId, user.ClientId.ToString());
        return baseClaims.Append(clientIdClaim).ToArray();
    }
}
