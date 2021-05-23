using System.Collections.Generic;
using System.Linq;
using Pingo.Models;

namespace Pingo.Services
{
    public class RoomManager
    {
        public RoomManager()
        {
            Rooms = new List<Room>();
        }

        public List<Room> Rooms { get; set; }

        public Room GetRoomByUserId(string userId)
        {
            var room = Rooms.FirstOrDefault(x => x.Users.Contains(userId));
            return room;
        }
    }
}