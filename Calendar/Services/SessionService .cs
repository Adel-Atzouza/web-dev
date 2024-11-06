using Microsoft.AspNetCore.Http;

namespace Calendar.Services
{
    public class SessionService : ISessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        //used for the SESSION_KEY, for admin or user
        public void SetSessionKey(string key, string value)
        {
            _httpContextAccessor.HttpContext?.Session.SetString(key, value);
        }

        public string? GetSessionKey(string key)
        {
            return _httpContextAccessor.HttpContext?.Session.GetString(key);
        }

        public void RemoveSessionKey(string key)
        {
            _httpContextAccessor.HttpContext?.Session.Remove(key);
        }
    }

}
