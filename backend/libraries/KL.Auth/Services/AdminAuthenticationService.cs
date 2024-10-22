using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using KL.Auth.Models.User;
using KL.Auth.Services.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KL.Auth.Services
{
    public class AdminAuthenticationService<TUser,TId> : IAdminAuthenticationService
        where TUser : IdentityUser<TId> where TId : IEquatable<TId>
    {
        private readonly SignInManager<TUser> _signInManager;
        private readonly UserManager<TUser> _userManager;
        private readonly IJwtTokenGenerator<TUser, TId> _jwtTokenGenerator;

        public AdminAuthenticationService(SignInManager<TUser> signInManager,
                                    UserManager<TUser> userManager,
                                    IJwtTokenGenerator<TUser, TId> jwtTokenGenerator)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> Login(LoginInputModel model)
        {
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!result.Succeeded)
            {
                throw new Exception("Invalid login attempt.");
            }

            var user = await _userManager.Users.SingleOrDefaultAsync(u => u.Email == model.Email);
            IList<string> userRoles = await _userManager.GetRolesAsync(user);

            return _jwtTokenGenerator.Generate(user, userRoles);
        }
    }
}