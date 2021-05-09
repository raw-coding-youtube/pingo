using System.Linq;
using Microsoft.AspNetCore.SignalR;

namespace Pingo.CustomAuthR
{
    public class UserIdProvider : IUserIdProvider
    {
        public string GetUserId(HubConnectionContext connection)
        {
            var claims = connection.User.Claims.FirstOrDefault(x => x.Type == "UserId");
            return claims.Value.ToString();
        }
    }
}