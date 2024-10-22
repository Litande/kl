using KL.Auth.Controllers;
using KL.Auth.Services;
using Microsoft.AspNetCore.Mvc;

namespace KL.Manager.API.Controllers;

public class UserController(IAdminAuthenticationService adminAuthenticationService) : AccountController(adminAuthenticationService)
{
    public override async Task<IActionResult> Me()
    {
        return Ok("Hello from Manager API");
    }
}