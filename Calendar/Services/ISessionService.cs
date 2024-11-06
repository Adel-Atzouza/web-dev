using Microsoft.AspNetCore.Http;

namespace Calendar.Services
{
    public interface ISessionService
    {
        void SetSessionKey(string key, string value);
        string? GetSessionKey(string key);
        void RemoveSessionKey(string key);
    }

}
