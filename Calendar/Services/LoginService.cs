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
        var admin = _context.Admin.FirstOrDefault(a => a.UserName == username);
        if (admin == null) return LoginStatus.IncorrectUsername;
        if (admin.Password == EncryptionHelper.EncryptPassword(inputPassword)) return LoginStatus.Success;

        return LoginStatus.IncorrectPassword;
    }
}