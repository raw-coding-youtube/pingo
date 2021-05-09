using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pingo.Hubs;

namespace Pingo.Controllers
{
    [ApiController]
    [Route("/api/auth")]
    public class AuthController : ControllerBase
    {

        [HttpGet]
        public async Task<IActionResult> DoAsync()
        {
            var guid = Guid.NewGuid().ToString();
            var claim = new Claim("UserId", guid);

            ClaimsIdentity identity = new ClaimsIdentity(new[] { claim }, "my_scheme");
            
            await HttpContext.SignInAsync("my_scheme", new ClaimsPrincipal(identity));

            return Ok();
        }


    }
}



