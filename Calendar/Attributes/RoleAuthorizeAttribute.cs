using Calendar.Services;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Calendar.Attributes
{
    public class RoleAuthorizeAttribute : Attribute, IAuthorizationFilter
    {
        private readonly string _role;

        public RoleAuthorizeAttribute(string role)
        {
            _role = role;  // "Admin" or "User"
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            // Check the session for admin or user login
            var adminLoggedIn = context.HttpContext.Session.GetString(SESSION_KEY.adminLoggedIn.ToString());
            var userLoggedIn = context.HttpContext.Session.GetString(SESSION_KEY.userLoggedIn.ToString());

            // If the requested role is Admin
            if (_role == "Admin" && adminLoggedIn == null)
            {
                context.Result = new UnauthorizedResult();
            }
            // If the requested role is User (and it could be either admin or regular user)
            else if (_role == "User" && userLoggedIn == null && adminLoggedIn == null)
            {
                context.Result = new UnauthorizedResult();
            }
        }
    }

}
