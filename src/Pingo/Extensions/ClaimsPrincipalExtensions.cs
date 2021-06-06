using System.Linq;
using Pingo.Models;
using Microsoft.AspNetCore.Http;

namespace Pingo.Extensions
{
    public static class ClaimsPrincipalExtensions
    {

        public static string UserId(this HttpContext ctx)
        {
            return ctx.User.Claims.FirstOrDefault(x => x.Type == "UserId").Value;
        }

    }
}