using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Pingo.Hubs
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Checks if message message is correct and
        /// Will send a message to all clients
        /// </summary>
        public async Task SendMessage(string user, string message)
        {
            const string correctMessage = "hello";

            if (message == correctMessage)
            {
                // Right now only returns the same message, so the user who did it correct will see it twice
                // ToDo: Implement a new function for tchecked client
                // ToDo: Break here, don't send a message to everyone
                await Clients.User(Context.User.Identity.Name).SendAsync("ReceiveMessage", user, message);
            }
           
            // Client.All should send the command "ReceiveMessage" to all clients ==> it will aslo send it to itself
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}