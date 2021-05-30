using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pingo.Hubs;
using Pingo.Models;
using Pingo.Services;

namespace Pingo.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly RoomManager _manager;
        private IHubContext<ChatHub> _chatHub;

        public RoomController(RoomManager manager, IHubContext<ChatHub> hub)
        {
            _manager = manager;
            _chatHub = hub;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            return Ok(_manager.Rooms);
        }

        [HttpGet("{id}")]
        public IActionResult GetRoom(int id)
        {
            return Ok(_manager.Rooms.FirstOrDefault(x => x.Id == id));
        }

        [HttpPost("{id}")]
        public IActionResult Create(int id)
        {
            _manager.Rooms.Add(new Room { Id = id });
            return Ok();
        }

        [HttpGet("my")]
        public IActionResult MyRoom()
        {
            var userId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "UserId").Value;

            var MyRoom = _manager.Rooms.FirstOrDefault(x => x.Users.Contains(userId));
            return Ok(MyRoom);
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> StartGameAsync(int id)
        {
            var room = _manager.Rooms.FirstOrDefault(x => x.Id == id);
            room.Started = true;

            await _chatHub.Clients.Group(room.Id.ToString()).SendAsync("ReloadRoom", new
            {
                roomId = room.Id,
                users = room.Users,
                started = room.Started,
            });

            return Ok();
        }
    }
}



