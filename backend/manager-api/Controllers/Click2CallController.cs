﻿using Microsoft.AspNetCore.Mvc;
using Plat4Me.DialClientApi.Application.Models.Requests.Click2Call;

namespace Plat4Me.DialClientApi.Controllers;

[Route("click2call")]
public class Click2CallController : Controller
{

    public Click2CallController()
    {
    }

    [HttpPost]
    public async Task<IActionResult> Click2Call(
        [FromBody] CallRequest request,
        CancellationToken ct = default)
    {        
        await Task.Delay(1000);
        var response = Guid.NewGuid().ToString();
        return Ok(response);
    }
   
}
