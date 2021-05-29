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
    public class ProfileRoute
    {
        public const string Signup = "rpc/locating-app/account/signup";
        public const string Login = "rpc/locating-app/account/login";
        public const string Logged = "rpc/locating-app/account/logged";
        public const string GetForWeb = "rpc/locating-app/profile-web/get";
        public const string Get = "rpc/locating-app/profile/get";
        public const string GetDraft = "rpc/locating-app/profile/get-draft";
        public const string Update = "rpc/locating-app/profile/update";
        public const string SaveImage = "rpc/locating-app/profile/save-image";
        public const string ChangePassword = "rpc/locating-app/profile/change-password";
        public const string ForgotPassword = "rpc/locating-app/profile/forgot-password";
        public const string VerifyOtpCode = "rpc/locating-app/profile/verify-otp-code";
        public const string RecoveryPassword = "rpc/locating-app/profile/recovery-password";
        public const string SingleListSex = "rpc/locating-app/profile/single-list-sex";
        public const string SingleListProvince = "rpc/locating-app/profile/single-list-province";
    }
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private IAppUserService AppUserService;
        private ISexService SexService;
        private ICurrentContext CurrentContext;
        public ProfileController(
            IAppUserService AppUserService,
            ISexService SexService,
            ICurrentContext CurrentContext
            )
        {
            this.AppUserService = AppUserService;
            this.SexService = SexService;
            this.CurrentContext = CurrentContext;
        }

        [AllowAnonymous]
        [Route(ProfileRoute.Signup), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Signup([FromBody] AppUser_SignupDTO AppUser_SignupDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Username = AppUser_SignupDTO.Username,
                Password = AppUser_SignupDTO.Password,
                DisplayName = AppUser_SignupDTO.DisplayName,
                SexId = AppUser_SignupDTO.SexId,
                Birthday = AppUser_SignupDTO.Birthday,
                Email = AppUser_SignupDTO.Email,
                Phone = AppUser_SignupDTO.Phone,
            };
            AppUser = await AppUserService.Create(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [AllowAnonymous]
        [Route(ProfileRoute.Login), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> Login([FromBody] AppUser_LoginDTO AppUser_LoginDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            AppUser AppUser = new AppUser
            {
                Username = AppUser_LoginDTO.Username,
                Password = AppUser_LoginDTO.Password,
                BaseLanguage = "vi",
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.Login(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);

            if (AppUser.IsValidated)
            {
                Response.Cookies.Append("Token", AppUser.Token);
                AppUser_AppUserDTO.Token = AppUser.Token;
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoute.Logged), HttpPost]
        public bool Logged()
        {
            return true;
        }

        [Route(ProfileRoute.ChangePassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ChangePassword([FromBody] AppUser_ProfileChangePasswordDTO AppUser_ProfileChangePasswordDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            this.CurrentContext.UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = CurrentContext.UserId,
                Password = AppUser_ProfileChangePasswordDTO.OldPassword,
                NewPassword = AppUser_ProfileChangePasswordDTO.NewPassword,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.ChangePassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        #region Forgot Password
        [AllowAnonymous]
        [Route(ProfileRoute.ForgotPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> ForgotPassword([FromBody] AppUser_ForgotPassword AppUser_ForgotPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_ForgotPassword.Email,
            };
            AppUser.BaseLanguage = CurrentContext.Language;

            AppUser = await AppUserService.ForgotPassword(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                return AppUser_AppUserDTO;
            }
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [AllowAnonymous]
        [Route(ProfileRoute.VerifyOtpCode), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> VerifyCode([FromBody] AppUser_VerifyOtpDTO AppUser_VerifyOtpDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            AppUser AppUser = new AppUser
            {
                Email = AppUser_VerifyOtpDTO.Email,
                OtpCode = AppUser_VerifyOtpDTO.OtpCode,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.VerifyOtpCode(AppUser);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
            {
                HttpContext.Response.Cookies.Append("Token", AppUser.Token);
                return AppUser_AppUserDTO;
            }

            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoute.RecoveryPassword), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> RecoveryPassword([FromBody] AppUser_RecoveryPassword AppUser_RecoveryPassword)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            var UserId = ExtractUserId();
            AppUser AppUser = new AppUser
            {
                Id = UserId,
                Password = AppUser_RecoveryPassword.Password,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            AppUser = await AppUserService.RecoveryPassword(AppUser);
            if (AppUser == null)
                return Unauthorized();
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }
        #endregion

        //[Route(ProfileRoot.SaveImage), HttpPost]
        //public async Task<ActionResult<string>> SaveImage(IFormFile file)
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    MemoryStream memoryStream = new MemoryStream();
        //    file.CopyTo(memoryStream);
        //    Image Image = new Image
        //    {
        //        Name = file.FileName,
        //        Content = memoryStream.ToArray()
        //    };
        //    CurrentContext.Token = Request.Cookies["Token"];
        //    string str = await AppUserService.SaveImage(Image);
        //    return str;
        //}

        [Route(ProfileRoute.GetForWeb), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetForWeb()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }

        [Route(ProfileRoute.Get), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> GetMe()
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);
            var UserId = ExtractUserId();
            AppUser AppUser = await AppUserService.Get(UserId);
            AppUser_AppUserDTO AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            return AppUser_AppUserDTO;
        }

        //[Route(ProfileRoot.GetDraft), HttpPost]
        //public async Task<ActionResult<AppUser_AppUserDTO>> GetDraft()
        //{
        //    if (!ModelState.IsValid)
        //        throw new BindException(ModelState);
        //    var UserId = ExtractUserId();
        //    AppUser AppUser = await AppUserService.Get(UserId);

        //    return new AppUser_AppUserDTO(AppUser);
        //}

        [Route(ProfileRoute.Update), HttpPost]
        public async Task<ActionResult<AppUser_AppUserDTO>> UpdateMe([FromBody] AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            if (!ModelState.IsValid)
                throw new BindException(ModelState);

            this.CurrentContext.UserId = ExtractUserId();
            AppUser AppUser = ConvertDTOToEntity(AppUser_AppUserDTO);
            AppUser.Id = CurrentContext.UserId;
            AppUser = await AppUserService.Update(AppUser);
            AppUser_AppUserDTO = new AppUser_AppUserDTO(AppUser);
            if (AppUser.IsValidated)
                return AppUser_AppUserDTO;
            else
                return BadRequest(AppUser_AppUserDTO);
        }

        [Route(ProfileRoute.SingleListSex), HttpPost]
        public async Task<List<AppUser_SexDTO>> SingleListSex([FromBody] AppUser_SexFilterDTO AppUser_SexFilterDTO)
        {
            SexFilter SexFilter = new SexFilter();
            SexFilter.Skip = 0;
            SexFilter.Take = 20;
            SexFilter.OrderBy = SexOrder.Id;
            SexFilter.OrderType = OrderType.ASC;
            SexFilter.Selects = SexSelect.ALL;
            SexFilter.Id = AppUser_SexFilterDTO.Id;
            SexFilter.Code = AppUser_SexFilterDTO.Code;
            SexFilter.Name = AppUser_SexFilterDTO.Name;
            List<Sex> Sexes = await SexService.List(SexFilter);
            List<AppUser_SexDTO> AppUser_SexDTOs = Sexes
                .Select(x => new AppUser_SexDTO(x)).ToList();
            return AppUser_SexDTOs;
        }

        private long ExtractUserId()
        {
            return long.TryParse(User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
        }
        private AppUser ConvertDTOToEntity(AppUser_AppUserDTO AppUser_AppUserDTO)
        {
            AppUser AppUser = new AppUser();
            AppUser.Id = AppUser_AppUserDTO.Id;
            AppUser.Username = AppUser_AppUserDTO.Username;
            AppUser.Password = AppUser_AppUserDTO.Password;
            AppUser.DisplayName = AppUser_AppUserDTO.DisplayName;
            //AppUser.Avatar = AppUser_AppUserDTO.Avatar;
            AppUser.Birthday = AppUser_AppUserDTO.Birthday;
            AppUser.Email = AppUser_AppUserDTO.Email;
            AppUser.Phone = AppUser_AppUserDTO.Phone;
            AppUser.SexId = AppUser_AppUserDTO.SexId;
            AppUser.Sex = AppUser_AppUserDTO.Sex == null ? null : new Sex
            {
                Id = AppUser_AppUserDTO.Sex.Id,
                Code = AppUser_AppUserDTO.Sex.Code,
                Name = AppUser_AppUserDTO.Sex.Name,
            };
            AppUser.BaseLanguage = CurrentContext.Language;
            return AppUser;
        }
    }
}
