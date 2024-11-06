using System.Text;
using Microsoft.AspNetCore.Mvc;
using Calendar.Services;

namespace Calendar.Controllers;


[Route("api/v1/Login")]
public class LoginController : Controller
{
    private readonly ILoginService _loginService;
    

    public LoginController(ILoginService loginService)
    {
        _loginService = loginService;
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
            HttpContext.Session.SetString(ADMIN_SESSION_KEY.adminLoggedIn.ToString(), loginBody.Username);
            return Ok("Login successful");
        }

        return loginStatus == LoginStatus.IncorrectUsername
            ? Unauthorized("Incorrect username")
            : Unauthorized("Incorrect password");
    }


    [HttpGet("IsAdminLoggedIn")]
    public IActionResult IsAdminLoggedIn()
    {
        // TODO: This method should return a status 200 OK when logged in, else 403, unauthorized
        var adminLoggedIn = HttpContext.Session.GetString(ADMIN_SESSION_KEY.adminLoggedIn.ToString());
        if (adminLoggedIn != null)
        {
            return Ok($"You are logged in as admin user \"{adminLoggedIn}\"");
        }

        return Unauthorized("You are not logged in");
    }

    [HttpGet("Logout")]
    public IActionResult Logout()
    {
        if (HttpContext.Session.GetString(ADMIN_SESSION_KEY.adminLoggedIn.ToString()) == null)
        {
            return Unauthorized("You are not logged in");
        }
        HttpContext.Session.Remove(ADMIN_SESSION_KEY.adminLoggedIn.ToString());
        
        return Ok("Logged out");
    }

}

public class LoginBody
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

