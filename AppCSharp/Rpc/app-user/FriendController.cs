﻿using LocatingApp.Common;
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
        public const string GetFriendFromContact = "rpc/locating-app/friend/get-friend-from-contact";
    }
    public class FriendController : RpcController
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

        [Route(FriendRoute.GetFriendFromContact), HttpPost]
        public async Task<ActionResult<List<AppUser_AppUserDTO>>> GetFriendFromContact([FromBody] List<AppUser_UserPhoneDTO> AppUser_UserPhoneDTOs)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            List<AppUser_AppUserDTO> AppUser_AppUserDTOs = new List<AppUser_AppUserDTO>();
            foreach (var AppUser_UserPhoneDTO in AppUser_UserPhoneDTOs)
            {
                var AppUser = await AppUserService.GetFriendFromContact(AppUser_UserPhoneDTO.Phone);
                AppUser_AppUserDTOs.Add(new AppUser_AppUserDTO(AppUser));
            }
            return AppUser_AppUserDTOs;
        }

        [Route(FriendRoute.SendFriendRequest), HttpPost]
        public async Task<ActionResult<AppUser_AppUserAppUserMappingDTO>> SendFriendRequest([FromBody]AppUser_AppUserAppUserMappingDTO AppUser_AppUserAppUserMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!await HasPermission(AppUser_AppUserAppUserMappingDTO.AppUserId))
                return Forbid();
            
            var AppUserAppUserMapping = ConvertDTOToEntity(AppUser_AppUserAppUserMappingDTO);
            AppUserAppUserMapping = await AppUserService.SendFriendRequest(AppUserAppUserMapping);
            AppUser_AppUserAppUserMappingDTO = new AppUser_AppUserAppUserMappingDTO(AppUserAppUserMapping);
            return AppUser_AppUserAppUserMappingDTO;
        }

        [Route(FriendRoute.AcceptFriendRequest), HttpPost]
        public async Task<ActionResult<AppUser_AppUserAppUserMappingDTO>> AcceptFriendRequest([FromBody] AppUser_AppUserAppUserMappingDTO AppUser_AppUserAppUserMappingDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            if (!await HasPermission(AppUser_AppUserAppUserMappingDTO.FriendId))
                return Forbid();

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

        public async Task<bool> HasPermission(long UserId)
        {
            AppUser CurrentUser = await AppUserService.Get(CurrentContext.UserId);
            if (CurrentContext.RoleId == RoleEnum.Admin.Id)
                return true;
            else return CurrentUser.Id == UserId;
        }
    }
}
