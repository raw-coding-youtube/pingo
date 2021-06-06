using System.Collections.Generic;

namespace Pingo.Models
{
    public class LobbyRoomViewModel
    {

        public LobbyRoomViewModel(Room room, string userId)
        {
            RoomId = room.Id;
            Users = room.Users;
            Started = room.Started;
            IsAdmin = userId == room.Admin;
        }

        public int RoomId { get; set; }
        public List<string> Users { get; set; }
        public bool Started { get; set; }
        public bool IsAdmin { get; set; }
    }
}