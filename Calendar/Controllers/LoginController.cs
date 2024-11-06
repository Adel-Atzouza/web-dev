using System.Text;
using Microsoft.AspNetCore.Mvc;
using Calendar.Services;
using Calendar.Models;

namespace Calendar.Controllers;


[Route("api/v1/Login")]
public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    private readonly ISessionService _sessionService;


    public LoginController(ILoginService loginService, ISessionService sessionService)
    {
        _loginService = loginService;
        _sessionService = sessionService;
    }

    [HttpPost("Register")]
    public async Task<IActionResult> Register([FromBody] RegisterUserDto registerDto)
    {
        if (registerDto == null || string.IsNullOrEmpty(registerDto.UserName) || string.IsNullOrEmpty(registerDto.Password))
        {
            return BadRequest("Username and password are required.");
        }

        var result = await _loginService.RegisterUser(registerDto);

        if (result == "Username already exists")
        {
            return Conflict(result); // 409 Conflict for duplicate usernames
        }

        return Ok(result);
    }


    [HttpPost("Login")]
    public IActionResult Login([FromBody] LoginBody loginBody)
    {
        // TODO: Implement login method
        if (loginBody == null || string.IsNullOrEmpty(loginBody.Username) || string.IsNullOrEmpty(loginBody.Password))
        {
            return BadRequest("Username or password is missing");
        }

        var loginStatus = _loginService.CheckPassword(loginBody.Username, loginBody.Password);
        if (loginStatus == LoginStatus.Success)
        {
            HttpContext.Session.SetString(SESSION_KEY.adminLoggedIn.ToString(), loginBody.Username);
            return Ok("Login successful");
        }

        return loginStatus == LoginStatus.IncorrectUsername
            ? Unauthorized("Incorrect username")
            : Unauthorized("Incorrect password");
    }


    [HttpGet("IsAdminLoggedIn")]
    public IActionResult IsAdminLoggedIn()
    {
        //This method returns a status 200 OK when logged in, else 403, unauthorized
        var adminLoggedIn = _sessionService.GetSessionKey(SESSION_KEY.adminLoggedIn.ToString());
        if (adminLoggedIn != null)
        {
            return Ok(adminLoggedIn);
        }
        return Unauthorized();

    }

    [HttpGet("Logout")]
    private IActionResult Logout()
    {
        var sessionKey = _sessionService.GetSessionKey(SESSION_KEY.adminLoggedIn.ToString());
        if (sessionKey == null)
        {
            return Unauthorized("You are not logged in");
        }

        _sessionService.RemoveSessionKey(SESSION_KEY.adminLoggedIn.ToString());
        return Ok("Logged out");
    }
}

    public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

