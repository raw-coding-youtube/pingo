using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pingo.Hubs
{
    public class ChatHub : Hub
    {
        private readonly RoomManager _manager;
        private readonly ILogger<ChatHub> _logger;
        public ChatHub(RoomManager manager, ILogger<ChatHub> logger)
        {
            _manager = manager;
            _logger = logger;
        }

        public Task SendCoordinate(
            int roomId,
            int xStartPosition,
            int yStartPosition,
            int toX,
            int toY,
            string color,
            int pickerValue
            )
        {
            return Clients.Group(roomId.ToString()).SendAsync("ReceiveCoordinate", xStartPosition, yStartPosition, toX, toY, color, pickerValue);
        }
        public Task SendClearEvent(int roomId)
        {
            return Clients.Group(roomId.ToString()).SendAsync("ReceiveClearEvent");
        }

        public async Task JoinRoom(int id)
        {
            try
            {
                var room = _manager.Rooms.FirstOrDefault(x => x.Id == id);
                var userId = Context.UserIdentifier;

                if (!room.Users.Any(x => x == userId))
                {
                    room.Users.Add(userId);
                }

                await Groups.AddToGroupAsync(Context.ConnectionId, room.Id.ToString());

                await Clients.Caller.SendAsync("JoinResponse", room.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
            }
        }
    }
}