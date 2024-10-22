using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KL.Auth.Models.User;
using KL.Auth.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KL.Auth.Services
{
    public class AdminAuthenticationService<TUser, TId>(
        SignInManager<TUser> signInManager,
        UserManager<TUser> userManager,
        IJwtTokenGenerator<TUser, TId> jwtTokenGenerator)
        : IAdminAuthenticationService
        where TUser : IdentityUser<TId>
        where TId : IEquatable<TId>
    {
        public async Task<string> Login(LoginInputModel model)
        {
            var result = await signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid login attempt.");
            }

            var user = await userManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            var userRoles = await userManager.GetRolesAsync(user);

            return jwtTokenGenerator.Generate(user, userRoles);
        }
    }
}