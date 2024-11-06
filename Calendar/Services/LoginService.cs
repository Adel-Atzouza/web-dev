using Calendar.Controllers;
using Calendar.Models;
using Calendar.Utils;

namespace Calendar.Services;

public enum LoginStatus { IncorrectPassword, IncorrectUsername, Success }

public enum SESSION_KEY { adminLoggedIn, userLoggedIn }

public class LoginService : ILoginService
{

    private readonly DatabaseContext _context;
    private readonly ISessionService _sessionService;

    public LoginService(DatabaseContext context, ISessionService sessionService)
    {
        _context = context;
        _sessionService = sessionService;
    }

    public LoginStatus CheckPassword(string username, string inputPassword)
    {
        //if the username is from a user, this var user will be filled. this by default means that admin is empty
        var user = _context.User.FirstOrDefault(_ => _.UserName == username);
        var admin = _context.Admin.FirstOrDefault(a => a.UserName == username);
        if (user == null && admin == null) return (LoginStatus.IncorrectUsername);

        //check whether user or admin name is found
        if (admin == null)
        {
            //check their password | even though intellisense says user could be null, we already checked that it can't!
            if (user?.Password == EncryptionHelper.EncryptPassword(inputPassword)) 
            {
                //sets the session key to user since the user logs in here.
                _sessionService.SetSessionKey(SESSION_KEY.userLoggedIn.ToString(), username);
                return (LoginStatus.Success); 
            }
        }
        else
        {
            if (admin.Password == EncryptionHelper.EncryptPassword(inputPassword)) 
            {
                //sets the session key to admin since the admin logs in here.
                _sessionService.SetSessionKey(SESSION_KEY.adminLoggedIn.ToString(), username);
                return (LoginStatus.Success);
            }
        }
        //if the code ever gets here, it means no matching passwords were found
        return (LoginStatus.IncorrectPassword);
    }

    //RegisterUserDto is a model specifically designed for this process.
    public async Task<string> RegisterUser(RegisterUserDto registerDto)
    {
        //check if username already exists
        var existingUser = _context.User.FirstOrDefault(u => u.UserName == registerDto.UserName);
        if (existingUser != null)
        {
            return "Username already exists";
        }

        //make the password hashed so it's safe in the database
        var hashedPassword = EncryptionHelper.EncryptPassword(registerDto.Password);

        //make a user object from the registerDto data, so we can put it in the database
        var newUser = new User
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.Email,
            UserName = registerDto.UserName,
            Password = hashedPassword,

            //because of these fields, we need to use the Dto, here we just add them as empty, since they are required!
            RecuringDays = "",
            Attendances = new List<Attendance>(),
            Event_Attendances = new List<Event_Attendance>()
        };

        //add to the database
        _context.User.Add(newUser);
        await _context.SaveChangesAsync();

        return "User registered successfully";
    }

}