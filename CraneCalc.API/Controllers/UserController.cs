using CraneCalc.Application.Features.User.Commands.Login;
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
    public async Task<IActionResult> Register([FromBody] RegisterCommand request, CancellationToken ct)
    {
        await mediator.Send(request, ct);
        
        return NoContent();
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand request, CancellationToken ct)
    {
        var token = await mediator.Send(request, ct);
        
        if(token is null) 
            return NotFound();
        
        Response.Cookies.Append(TokenName.Cookie, token);
        
        return Ok(token);
    }

    [Authorize]
    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser([FromBody] UpdateCommand request, CancellationToken ct)
    {
        var token = await mediator.Send(request, ct);
        
        if(token is null)
            return NotFound();
        
        Response.Cookies.Append(TokenName.Cookie, token);
        
        return Ok(token);
    }

    [Authorize]
    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete(TokenName.Cookie);
        
        return NoContent();
    }
}