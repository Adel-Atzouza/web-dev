using Calendar.Models;
using Calendar.Utils;

namespace Calendar.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }

public enum ADMIN_SESSION_KEY { adminLoggedIn }

public class LoginService : ILoginService
{

    private readonly DatabaseContext _context;

    public LoginService(DatabaseContext context)
    {
        _context = context;
    }

    public LoginStatus CheckPassword(string username, string inputPassword)
    {
        // TODO: Make this method check the password with what is in the database
        return LoginStatus.IncorrectPassword;
    }
}