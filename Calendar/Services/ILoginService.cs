using Calendar.Models;

namespace Calendar.Services;

public interface ILoginService
{
    Task<string> RegisterUser(RegisterUserDto registerDto);
    public LoginStatus CheckPassword(string username, string inputPassword);
}