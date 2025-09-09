using Microsoft.AspNetCore.SignalR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Blog.Helpers
{
    public class NotificationHub : Hub
    {
        public async Task SendNotificationToUser(string userId, string message)
        {
            await Clients.User(userId).SendAsync("ReceiveNotification", message);
        }

        public async Task BroadcastNotification(string message)
        {
            await Clients.All.SendAsync("ReceiveNotification", message);
        }


        private readonly ILogger<NotificationHub> _logger;

        

      
    }
}

