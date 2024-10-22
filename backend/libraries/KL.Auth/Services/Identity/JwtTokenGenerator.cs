using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace KL.Auth.Services.Identity
{
    public class JwtTokenGenerator<TUser, TId> : IJwtTokenGenerator<TUser, TId>
        where TUser : IdentityUser<TId> where TId : IEquatable<TId>
    {
        private readonly JwtConfigurations _jwtConfigurations;

        public JwtTokenGenerator(IOptions<JwtConfigurations> jwtConfigurations)
        {
            _jwtConfigurations = jwtConfigurations.Value;
        }

        public virtual string Generate(TUser user, IList<string> userRoles)
        {
            var roles = userRoles.Select(r => new Claim(ClaimTypes.Role, r)).ToArray();

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtConfigurations.Key);
            var claims = new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
                }
                .Union(roles)
                .Union(GetCustomCaliClaims(user));
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(_jwtConfigurations.ExpirationInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        protected virtual Claim[] GetCustomCaliClaims(TUser user)
        {
            return Array.Empty<Claim>();
        }
    }

    public interface IJwtTokenGenerator<in TUser, TId>
        where TUser : IdentityUser<TId>
        where TId : IEquatable<TId>
    {
        public string Generate(TUser user, IList<string> userRoles);
    }
}