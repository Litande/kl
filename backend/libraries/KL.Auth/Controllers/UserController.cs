using System.Threading.Tasks;
using KL.Auth.Models.User;
using KL.Auth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KL.Auth.Controllers
{
    [ApiController]
    [Route("user")]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly IAdminAuthenticationService _adminAuthenticationService;

        public AccountController(IAdminAuthenticationService adminAuthenticationService)
        {
            _adminAuthenticationService = adminAuthenticationService;
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login([FromBody] LoginInputModel request)
        {
            return Ok(await _adminAuthenticationService.Login(request));
        }
        
        
        [HttpGet("me")]
        public virtual async Task<IActionResult> Me()
        {
            return Ok(new
            {
                name = "Me Name"
            });
        }
    }
}