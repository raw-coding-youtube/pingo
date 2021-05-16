using System.Collections.Generic;
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
    }
}