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

    [Authorize]
    [HttpGet("logout")]
    public IActionResult Logout()
    {
        // Access current jwt token.
        var token = Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(token))
        {
            return BadRequest(new { message = "JWT token is missing" });
        }
        // Add it to the blacklist.
        _userService.AddTokenToBlacklist(token);

        _logger.LogInformation("User logged out, token added to blacklist.");
        return Ok(new { message = $"{token} added to blacklist." });
    }
}