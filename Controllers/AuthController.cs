using HumanResourceManager.Auth;
using HumanResourceManager.Exceptions;
using HumanResourceManager.Models;
using HumanResourceManager.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HumanResourceManager.Controllers;

[ApiController]
[Route("/human_resource_manager/auth/")]
public class AuthController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IUserService userService, ILogger<AuthController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginRequest request)
    {
        var user = _userService.ValidateUser(request.Username, request.Password);

        if (user == null)
        {
            _logger.LogError("Login failed, the user could not be found.");
            throw new UserNotFoundException(request.Username, request.Password);
        }

        var token = JwtTokenGenerator.Generate(user.Username);

        _logger.LogInformation($"{user.Username} has logged in successfully.");
        return Ok(new { jwt = token });
    }
}