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
    public abstract class AccountController(IAdminAuthenticationService adminAuthenticationService) : ControllerBase
    {
        [HttpPost("login")]
        [AllowAnonymous]
        public virtual async Task<IActionResult> Login([FromBody] LoginInputModel request)
        {
            return Ok(await adminAuthenticationService.Login(request));
        }

        [HttpGet("me")]
        public abstract Task<IActionResult> Me();
    }
}