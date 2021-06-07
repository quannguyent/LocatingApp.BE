using LocatingApp.Entities;
using LocatingApp.Services.MAppUser;
using LocatingApp.Services.MLocationLog;
using Microsoft.AspNetCore.SignalR;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Hubs
{ 
    public class LocationHub : Hub
    {
        private ILocationLogService LocationLogService;
        private IAppUserService AppUserService;
        public LocationHub(ILocationLogService LocationLogService,
            IAppUserService AppUserService)
        {
            this.AppUserService = AppUserService;
            this.LocationLogService = LocationLogService;
        }
        public async Task SendLocation(long UserId)
        {
            var Friends = await AppUserService.ListFriends(UserId);
            var LocationLogs = Friends.Select(x => new
            {
                x.Id,
                x.Avatar,
                Longtitude = x.LocationLogs.Select(x => x.Longtitude).FirstOrDefault(),
                Latitude = x.LocationLogs.Select(x => x.Latitude).FirstOrDefault(),
            }).ToList();
            await Clients.Caller.SendAsync("ReceiveFriendLocation", LocationLogs);
        }
    }
}