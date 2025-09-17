using CraneCalc.Application.Contracts.Request;
using CraneCalc.Application.Interfaces.Repository;
using CraneCalc.Domain.Enums;
using CraneCalc.Domain.Models;
using Microsoft.AspNetCore.Mvc;

namespace CraneCalc.API.Controllers;

[ApiController]
[Route("api/user")]
public class UserController(IUserRepository repository) : ControllerBase
{
    [HttpGet("me")]
    public async Task<IActionResult> GetUser(CancellationToken ct)
    {
        var userId = Request.Cookies["userId"];
        var isAuthenticated = Request.Cookies["userIsAuthenticated"];
    
        if (userId == null || isAuthenticated == null || userId != isAuthenticated)
            return Unauthorized();
    
        var user = await repository.GetUserAsync(Convert.ToInt32(userId), ct);
        return Ok(user);
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterRequest request, CancellationToken ct)
    {
        var user = await repository.CreateUserAsync(new User
        {
            Login = request.Login,
            Password = request.Password,
            Role = Role.User
        }, ct);
        
        Response.Cookies.Append("userId", user.Id.ToString());
    
        return Ok(user);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request, CancellationToken ct)
    {
        var userId = Request.Cookies["userId"];
        
        if(userId == null)
            return Unauthorized();
        
        var user = await repository.GetUserAsync(Convert.ToInt32(userId), ct);
        
        if(user == null || user.Login != request.Login || user.Password != request.Password)
            return Unauthorized();
        
        Response.Cookies.Append("userId", user.Id.ToString());
        Response.Cookies.Append("userIsAuthenticated", user.Id.ToString());
        
        return Ok("logined");
    }

    [HttpPut("update")]
    public async Task<IActionResult> UpdateUser(UpdateUserRequest request, CancellationToken ct)
    {
        var userId = Request.Cookies["userID"];
        var isAuthenticated = Request.Cookies["userIsAuthenticated"];
        
        if(isAuthenticated!=userId)
            return Unauthorized();
        
        if (userId == null)
            return Unauthorized();
        
        var user = await repository.UpdateUserAsync(Convert.ToInt32(userId), new User
        {
            Login = request.Login,
            Password = request.Password,
            Role = Role.User
        }, ct);
        
        return Ok(user);
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("userId");
        Response.Cookies.Delete("userIsAuthenticated");
    
        return Ok();
    }
}