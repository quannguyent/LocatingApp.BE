using LocatingApp.Common;
using Microsoft.AspNetCore.Mvc;
using LocatingApp.Services.MAppUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using LocatingApp.Enums;
using System.IO;
using LocatingApp.Services.MSex;

namespace LocatingApp.Rpc.app_user
{
    public class FriendRoute
    {
        public const string SendFriendRequest = "rpc/locating-app/friend/send-friend-request";
        public const string AcceptFriendRequest = "rpc/locating-app/friend/accept-friend-request";
    }
    [Authorize]
    public class FriendController : ControllerBase
    {
        private IAppUserService AppUserService;
        private ISexService SexService;
        private ICurrentContext CurrentContext;
        public FriendController(
            IAppUserService AppUserService,
            ISexService SexService,
            ICurrentContext CurrentContext
            )
        {
            this.AppUserService = AppUserService;
            this.SexService = SexService;
            this.CurrentContext = CurrentContext;
        }

        [Route(FriendRoute.SendFriendRequest), HttpPost]
        public async Task<ActionResult<AppUser_AppUserAppUserMappingDTO>> SendFriendRequest([FromBody]AppUser_AppUserAppUserMappingDTO AppUser_AppUserAppUserMappingDTO)
        {
            var AppUserAppUserMapping = ConvertDTOToEntity(AppUser_AppUserAppUserMappingDTO);
            AppUserAppUserMapping = await AppUserService.SendFriendRequest(AppUserAppUserMapping);
            AppUser_AppUserAppUserMappingDTO = new AppUser_AppUserAppUserMappingDTO(AppUserAppUserMapping);
            return AppUser_AppUserAppUserMappingDTO;
        }

        [Route(FriendRoute.AcceptFriendRequest), HttpPost]
        public async Task<ActionResult<AppUser_AppUserAppUserMappingDTO>> AcceptFriendRequest([FromBody] AppUser_AppUserAppUserMappingDTO AppUser_AppUserAppUserMappingDTO)
        {
            var AppUserAppUserMapping = ConvertDTOToEntity(AppUser_AppUserAppUserMappingDTO);
            AppUserAppUserMapping = await AppUserService.AcceptFriendRequest(AppUserAppUserMapping);
            AppUser_AppUserAppUserMappingDTO = new AppUser_AppUserAppUserMappingDTO(AppUserAppUserMapping);
            return AppUser_AppUserAppUserMappingDTO;
        }

        private AppUserAppUserMapping ConvertDTOToEntity(AppUser_AppUserAppUserMappingDTO AppUser_AppUserAppUserMappingDTO)
        {
            AppUserAppUserMapping AppUserAppUserMapping = new AppUserAppUserMapping
            {
                AppUserId = AppUser_AppUserAppUserMappingDTO.AppUserId,
                FriendId = AppUser_AppUserAppUserMappingDTO.FriendId,
            };
            return AppUserAppUserMapping;
        }
    }
}
