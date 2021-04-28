using System.Linq;
using Microsoft.AspNetCore.Mvc;

namespace Pingo.Controllers
{
    [ApiController]
    [Route("/api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly RoomManager _manager;

        public RoomController(RoomManager manager)
        {
            _manager = manager;
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



