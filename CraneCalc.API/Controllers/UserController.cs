using CraneCalc.Application.Features.User.Commands.Login;
using CraneCalc.Application.Features.User.Commands.Logout;
using CraneCalc.Application.Features.User.Commands.Register;
using CraneCalc.Application.Features.User.Commands.Update;
using CraneCalc.Application.Features.User.Queries.Me;
using CraneCalc.Application.Options;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IMediator mediator) : ControllerBase
{
    [Authorize]
    [HttpGet("me")]
    public async Task<IActionResult> GetUser(CancellationToken ct)
    {
        var user = await mediator.Send(new MeQuery(), ct);
        
        return Ok(user);
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken ct)
    {
        await mediator.Send(request, ct);
        
        return NoContent();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken ct)
    {
        var result = await mediator.Send(request, ct);
        
        if(result is null) 
            return NotFound();
        
        Response.Cookies.Append(CookieNames.AccessToken, result.AccessToken, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddMinutes(15)
        });
    
        Response.Cookies.Append(CookieNames.RefreshToken, result.RefreshToken, new CookieOptions
        {
            HttpOnly = true,
            Expires = DateTimeOffset.UtcNow.AddDays(7)
        });
        
        return Ok(new
        {
            result.AccessToken
        });
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateCommand request, CancellationToken ct)
    {
        var token = await mediator.Send(request, ct);
        
        if(token is null)
            return NotFound();
        
        Response.Cookies.Append(CookieNames.AccessToken, token);
        
        return Ok(token);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(CancellationToken ct)
    {
        await mediator.Send(new LogoutCommand(), ct);
        
        Response.Cookies.Delete(CookieNames.AccessToken);
        Response.Cookies.Delete(CookieNames.RefreshToken);
        
        return NoContent();
    }
}