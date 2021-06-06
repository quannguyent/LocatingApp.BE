using LocatingApp.Entities;
using LocatingApp.Services.MLocationLog;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace LocatingApp.Hubs
{
    public class LocationHub : Hub
    {
        private ILocationLogService LocationLogService;
        public LocationHub(ILocationLogService LocationLogService)
        {
            this.LocationLogService = LocationLogService;
        }
        public async Task SendLocation(long UserId)
        {
            var LocationLog = new LocationLog();
            await Clients.Caller.SendAsync("ReceiveFriendLocation", LocationLog);
        }
    }
}