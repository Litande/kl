using System.Security.Claims;
using KL.Agent.API.Application.Common;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Plat4Me.Authentication.Services.Identity;

namespace KL.Agent.API.Persistent.Seed.Authentication;

public class JwtTokenGeneratorDial : JwtTokenGenerator<IdentityUser<long>, long>
{
    private readonly IDbContextFactory<DialDbContext> _dbContextFactory;

    public JwtTokenGeneratorDial(
        IOptions<JwtConfigurations> jwtConfigurations,
        IDbContextFactory<DialDbContext> dbContextFactory)
        : base(jwtConfigurations)
    {
        _dbContextFactory = dbContextFactory;
    }

    protected override Claim[] GetCustomCaliClaims(IdentityUser<long> identityUser)
    {
        using var context = _dbContextFactory.CreateDbContext();
        var user = context.Users
            .Where(r => r.UserId == identityUser.Id)
            .FirstOrDefault();

        var baseClaims = base.GetCustomCaliClaims(identityUser);

        if (user is null) return baseClaims;

        var clientIdClaim = new Claim(CustomClaimTypes.ClientId, user.ClientId.ToString());
        return baseClaims.Append(clientIdClaim).ToArray();
    }
}
