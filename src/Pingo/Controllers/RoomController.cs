using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pingo.Extensions;
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
        private readonly ILogger<RoomController> _logger;

        public RoomController(
            RoomManager manager, 
            IHubContext<ChatHub> hub,
            ILogger<RoomController> logger
            )
        {
            _manager = manager;
            _chatHub = hub;
            _logger = logger;
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
        public async Task<IActionResult> Create(int id, string connectionId)
        {
            var userId = HttpContext.UserId();

            Room room = new Room { Id = id };

            room.Users.Add(userId);
            _manager.Rooms.Add(room);

            await _chatHub.Groups.AddToGroupAsync(connectionId, room.Id.ToString());
            
            return Ok(new LobbyRoomViewModel(room, userId));
        }

        [HttpGet("my")]
        public IActionResult MyRoom()
        {
            var userId = HttpContext.UserId();

            var myRoom = _manager.Rooms
                .FirstOrDefault(x => x.Users.Contains(userId));

            if (myRoom != null)
                return Ok(new LobbyRoomViewModel(myRoom, userId));
            return Ok();
        }

        [HttpPut("{id}/start")]
        public async Task<IActionResult> StartGameAsync(int id)
        {
            var room = _manager.Rooms.FirstOrDefault(x => x.Id == id);
            room.Started = true;
            await _chatHub.Clients.Group(room.Id.ToString())
                .SendAsync(
                    "ReloadRoom",
                    new LobbyRoomViewModel(room, HttpContext.UserId())
                );

            return Ok();
        }

        [HttpPut("{roomid}/join")]
        public async Task<IActionResult> JoinRoom(int roomId, string connectionId)
        {
            _logger.LogInformation($"Room id: {roomId}, ConnectionID: {connectionId}");
            var room = _manager.Rooms.FirstOrDefault(x => x.Id == roomId);
            var userId = HttpContext.UserId();

            if (!room.Users.Any(x => x == userId))
            {
                room.Users.Add(userId);
            }

            await _chatHub.Groups.AddToGroupAsync(connectionId, room.Id.ToString());

            var vm = new LobbyRoomViewModel(room, userId);
            //await _chatHub.Clients.Caller.SendAsync("JoinResponse");
            await _chatHub.Clients.Group(room.Id.ToString())
               .SendAsync("ReloadRoom", vm);

            return Ok(vm);
        }
    }
}



