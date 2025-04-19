using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using SkillSync.UserService.Application.Contracts;
using SkillSync.UserService.Application.DTOs;

namespace SkillSync.UserService.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase{
    private readonly IUserService _userService;
    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest registerRequest)
    {
        if(!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var token = await _userService.RegisterAsync(registerRequest);
        return  Ok(new {token});
    }

    [HttpGet("ping")]
    public IActionResult Get()=> Ok("API LOADED!!");

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody]  LoginRequest loginRequest)
    {
        try
        {
            var response = await _userService.LoginAsync(loginRequest);
            return Ok(response);
        }
        catch (UnauthorizedAccessException ex)
        {
            
            return Unauthorized(new {message = ex.Message});
        }
    }  
}