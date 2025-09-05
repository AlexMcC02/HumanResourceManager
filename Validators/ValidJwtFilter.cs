using HumanResourceManager.Controllers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HumanResourceManager.Validators;

public class ValidJwtFilter : IActionFilter
{
    private readonly ApplicationDbContext _dbContext;
    private readonly ILogger<ValidJwtFilter> _logger;


    public ValidJwtFilter(ApplicationDbContext dbContext, ILogger<ValidJwtFilter> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public void OnActionExecuted(ActionExecutedContext context) { }

    public void OnActionExecuting(ActionExecutingContext context)
    {
        var token = context.HttpContext.Request.Headers.Authorization.ToString().Replace("Bearer ", "");

        var blackListedTokens = _dbContext.BlacklistedTokens.ToList();

        foreach (var blacklistedToken in blackListedTokens)
        {
            if (blacklistedToken.Token == token)
            {
                _logger.LogError("A blacklisted JWT token has been rejected.");   
                context.Result = new UnauthorizedObjectResult("JWT token has been blacklisted. You will need to login again.");
            }
        }
    }
}
