namespace Calendar.Services;

public interface ILoginService {
    public LoginStatus CheckPassword(string username, string inputPassword);
}