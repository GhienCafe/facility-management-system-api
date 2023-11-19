using Microsoft.AspNetCore.SignalR;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AppCore.Extensions;
using AppCore.Models;
using MainData.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MainData.Hubs
{
    public class NotificationHub : Hub
    {
        public NotificationHub()
        {
        }

        [HubMethodName("GetNotifications")]
        public async Task SendNotificationToClients()
        {
            try
            {
                var accessToken = Context.GetHttpContext()?.Request.Query["access_token"].ToString();
                
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(accessToken);

                var userIdClaim = jwtToken?.Claims?.FirstOrDefault(c => c.Type == AppClaimTypes.UserId);
                if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId))
                {
                    var dbContext = Context.GetHttpContext()?.RequestServices.GetRequiredService<DatabaseContext>();

                    if (dbContext != null)
                    {
                        var notifications = await dbContext.Notifications
                            .Where(x => x.UserId == userId)
                            .ToListAsync();

                        await Clients.All.SendAsync("ReceiveNotifications", notifications);
                    }
                }
                else
                {
                    // Handle the case when userIdClaim is null or user ID is not in the expected format
                    await Clients.All.SendAsync("ReceiveNotifications", null);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {}
            base.Dispose(disposing);
        }
    }
}