using HumanResourceManager.Models;

namespace HumanResourceManager.Services;

public interface IUserService
{
    User? ValidateUser(string username, string password);
    Task AddTokenToBlacklist(string token);
}

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public User? ValidateUser(string username, string password)
    {
        return _context.Users.FirstOrDefault(u => u.Username == username && u.Password == password);
    }

    public async Task AddTokenToBlacklist(string token)
    {
        var blackListedToken = new BlacklistedToken { Token = token };
        _context.BlacklistedTokens.Add(blackListedToken);
        await _context.SaveChangesAsync();
    }
}
    