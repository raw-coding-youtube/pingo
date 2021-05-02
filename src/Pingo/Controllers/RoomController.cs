using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Pingo.Hubs;

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
    }
}



