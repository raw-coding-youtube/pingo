using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Pingo.Models;
using Pingo.Services;
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

        public Task SendCoordinate(DrawEvent drawEvent)
        {
            var room = _manager.GetRoomByUserId(Context.UserIdentifier);
            room.DrawEvents.Add(drawEvent);
            return Clients.Group(room.Id.ToString()).SendAsync("ReceiveCoordinate", drawEvent);
        }
        public Task SendClearEvent()
        {
            var room = _manager.GetRoomByUserId(Context.UserIdentifier);
            return Clients.Group(room.Id.ToString()).SendAsync("ReceiveClearEvent");
        }

        public async Task JoinRoom(int roomId)
        {
            try
            {

                var room = _manager.Rooms.FirstOrDefault(x => x.Id == roomId);
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

        public Task GuessWord(string guessWord)
        {
            var room = _manager.GetRoomByUserId(Context.UserIdentifier);
            bool result = room.Word.Equals(guessWord, StringComparison.InvariantCultureIgnoreCase);
            return Clients.Group(room.Id.ToString()).SendAsync("GuessWordResponse", guessWord, result);
        }

        public async Task ReDraw()
        {
            var room = _manager.GetRoomByUserId(Context.UserIdentifier);
            foreach (var drawEvent in room.DrawEvents)
            {
                await Clients.Caller.SendAsync("ReceiveCoordinate", drawEvent);
            }
        }
    }
}