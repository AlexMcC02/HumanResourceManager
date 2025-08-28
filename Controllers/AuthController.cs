using HumanResourceManager.Auth;
using HumanResourceManager.Exceptions;
using HumanResourceManager.Models;
using HumanResourceManager.Services;
using Microsoft.AspNetCore.Mvc;

namespace HumanResourceManager.Controllers;

[ApiController]
[Route("/human_resource_manager/auth/")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;

    public AuthController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userService.ValidateUser(request.Username, request.Password);

        if (user == null)
        {
            throw new UserNotFoundException(request.Username, request.Password);
        }

        var token = JwtTokenGenerator.Generate(user.Username);

        return Ok(new { jwt = token });
    }
}