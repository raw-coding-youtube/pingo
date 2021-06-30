using System;
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

        [HttpPost]
        public async Task<IActionResult> Create(string connectionId)
        {
            var userId = HttpContext.UserId();

            var id = Guid.NewGuid().ToString();
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
        public async Task<IActionResult> StartGameAsync(string id)
        {
            var room = _manager.Rooms.FirstOrDefault(x => x.Id == id);
            room.Started = true;
            room.DrawingUser = room.Users[0];

            await _chatHub.Clients.Group(room.Id.ToString())
                .SendAsync("GameStarted");

            await _chatHub.Clients.Group(room.Id.ToString())
                .SendAsync("TurnUpdated", room.DrawingUser);

            return Ok(new LobbyRoomViewModel(room, HttpContext.UserId()));
        }

        [HttpPut("{roomid}/join")]
        public async Task<IActionResult> JoinRoom(string roomId, string connectionId)
        {
            _logger.LogInformation($"Room id: {roomId}, ConnectionID: {connectionId}");
            var room = _manager.Rooms.FirstOrDefault(x => x.Id == roomId);
            var userId = HttpContext.UserId();

            if (!room.Users.Any(x => x == userId))
            {
                room.Users.Add(userId);
                await _chatHub.Clients.Group(room.Id.ToString()).SendAsync("UserJoined", userId);
            }

            await _chatHub.Groups.AddToGroupAsync(connectionId, room.Id.ToString());

            return Ok(new LobbyRoomViewModel(room, userId));
        }
    }
}



