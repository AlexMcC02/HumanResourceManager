using HumanResourceManager.Models;

namespace HumanResourceManager.Services;

public interface IUserService
{
    User? ValidateUser(string username, string password);
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
}
    