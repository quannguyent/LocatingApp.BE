using LocatingApp.Common;
using LocatingApp.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using OfficeOpenXml;
using LocatingApp.Repositories;
using LocatingApp.Entities;
using LocatingApp.Enums;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using LocatingApp.Services.MMail;
using RestSharp;
using LocatingApp.Models;

namespace LocatingApp.Services.MAppUser
{
    public interface IAppUserService :  IServiceScoped
    {
        Task<int> Count(AppUserFilter AppUserFilter);
        Task<List<AppUser>> List(AppUserFilter AppUserFilter);
        Task<List<AppUser>> ListFriends(long AppUserId);
        Task<List<AppUser>> ListFriends(AppUserFilter AppUserFilter, long AppUserId);
        Task<List<AppUser>> ListFriendRequests(AppUserFilter AppUserFilter, long AppUserId);
        Task<AppUserAppUserMapping> SendFriendRequest(AppUserAppUserMapping AppUserAppUserMapping);
        Task<AppUserAppUserMapping> AcceptFriendRequest(AppUserAppUserMapping AppUserAppUserMapping);
        Task<AppUserAppUserMapping> DeleteFriend(AppUserAppUserMapping AppUserAppUserMapping);
        Task<AppUser> GetFriendFromContact(string Phone);
        Task<AppUser> Get(long Id);
        Task<AppUser> Create(AppUser AppUser);
        Task<AppUser> Update(AppUser AppUser);
        Task<AppUser> Delete(AppUser AppUser);
        Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers);
        Task<List<AppUser>> Import(List<AppUser> AppUsers);
        Task<AppUser> Login(AppUser AppUser);
        Task<AppUser> ChangePassword(AppUser AppUser);
        Task<AppUser> ForgotPassword(AppUser AppUser);
        Task<AppUser> VerifyOtpCode(AppUser AppUser);
        Task<AppUser> RecoveryPassword(AppUser AppUser);
        Task<AppUser> SaveImage(string path);
    }

    public class AppUserService : BaseService, IAppUserService
    {
        private IUOW UOW;
        private ILogging Logging;
        private ICurrentContext CurrentContext;
        private IAppUserValidator AppUserValidator;
        private IMailService MailService;
        private IConfiguration Configuration;

        public AppUserService(
            IUOW UOW,
            ICurrentContext CurrentContext,
            IAppUserValidator AppUserValidator,
            IMailService MailService,
            ILogging Logging,
            IConfiguration Configuration
        )
        {
            this.UOW = UOW;
            this.Logging = Logging;
            this.MailService = MailService;
            this.CurrentContext = CurrentContext;
            this.AppUserValidator = AppUserValidator;
            this.Configuration = Configuration;
        }
        public async Task<int> Count(AppUserFilter AppUserFilter)
        {
            try
            {
                int result = await UOW.AppUserRepository.Count(AppUserFilter);
                return result;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return 0;
        }

        public async Task<List<AppUser>> List(AppUserFilter AppUserFilter)
        {
            try
            {
                List<AppUser> AppUsers = await UOW.AppUserRepository.List(AppUserFilter);
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> ListFriends(long AppUserId)
        {
            try
            {
                var AppUser = await UOW.AppUserRepository.Get(AppUserId);
                List<long> FriendIds = new List<long>();
                foreach (var AppUserMapping in AppUser.AppUserAppUserMappingAppUsers)
                {
                    if (AppUser.AppUserAppUserMappingFriends.Exists(x => x.AppUserId == AppUserMapping.FriendId))
                    {
                        FriendIds.Add(AppUserMapping.FriendId);
                    }   
                }
                List<AppUser> AppUserFriends = await UOW.AppUserRepository.List(FriendIds);
                return AppUserFriends;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> ListFriends(AppUserFilter AppUserFilter, long AppUserId)
        {
            var FriendIds = (await ListFriends(AppUserId)).Select(x => x.Id).ToList();
            AppUserFilter.Id = FilterBuilder.Merge(new IdFilter { In = FriendIds }, AppUserFilter.Id);
            return await UOW.AppUserRepository.List(AppUserFilter);
        }

        public async Task<List<AppUser>> ListFriendRequests(AppUserFilter AppUserFilter, long AppUserId)
        {
            var CurrentUser = await UOW.AppUserRepository.Get(AppUserId);
            var FriendIds = (await ListFriends(AppUserId)).Select(x => x.Id).ToList();
            var FriendRequests = CurrentUser.AppUserAppUserMappingFriends.Select(x => x.AppUserId).ToList();
            AppUserFilter.Id = FilterBuilder.Merge(new IdFilter { In = FriendRequests, NotIn = FriendIds }, AppUserFilter.Id);
            return await UOW.AppUserRepository.List(AppUserFilter);
        }


        public async Task<AppUserAppUserMapping> SendFriendRequest(AppUserAppUserMapping AppUserAppUserMapping)
        {
            if (!await AppUserValidator.SendFriendRequest(AppUserAppUserMapping))
                return AppUserAppUserMapping;
            AppUser AppUser = await UOW.AppUserRepository.Get(AppUserAppUserMapping.AppUserId);
            AppUser.AppUserAppUserMappingAppUsers.Add(new AppUserAppUserMapping
            {
                AppUserId = AppUserAppUserMapping.AppUserId,
                FriendId = AppUserAppUserMapping.FriendId,
                CreatedAt = StaticParams.DateTimeNow,
            });
            await UOW.AppUserRepository.Update(AppUser);
            return AppUserAppUserMapping;
        }

        public async Task<AppUser> GetFriendFromContact(string Phone)
        {
            AppUser CurrentUser = await Get(CurrentContext.UserId);
            var FriendList = await ListFriends(CurrentUser.Id);
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Id = new IdFilter { NotIn = FriendList.Select(x => x.Id).ToList() },
                Phone = new StringFilter { Equal = Phone },
                Selects = AppUserSelect.ALL,
                Take = int.MaxValue,
                Skip = 0,
            };
            var AppUser = await UOW.AppUserRepository.List(AppUserFilter);
            return AppUser.FirstOrDefault();
        }

        public async Task<AppUserAppUserMapping> AcceptFriendRequest(AppUserAppUserMapping AppUserAppUserMapping)
        {
            if (!await AppUserValidator.AcceptFriendRequest(AppUserAppUserMapping))
                return AppUserAppUserMapping;
            AppUser AppUser = await UOW.AppUserRepository.Get(AppUserAppUserMapping.FriendId);
            AppUser.AppUserAppUserMappingAppUsers.Add(new AppUserAppUserMapping 
            {
                AppUserId = AppUserAppUserMapping.FriendId,
                FriendId = AppUserAppUserMapping.AppUserId,
                CreatedAt = StaticParams.DateTimeNow,
            });
            await UOW.AppUserRepository.Update(AppUser);
            return AppUserAppUserMapping;
        }

        public async Task<AppUserAppUserMapping> DeleteFriend(AppUserAppUserMapping AppUserAppUserMapping)
        {
            if (!await AppUserValidator.DeleteFriend(AppUserAppUserMapping))
                return AppUserAppUserMapping;
            AppUser AppUser = await UOW.AppUserRepository.Get(AppUserAppUserMapping.AppUserId);
            AppUser Friend = await UOW.AppUserRepository.Get(AppUserAppUserMapping.FriendId);
            await UOW.AppUserRepository.Update(AppUser);
            return AppUserAppUserMapping;
        }
        
        public async Task<AppUser> Get(long Id)
        {
            AppUser AppUser = await UOW.AppUserRepository.Get(Id);
            if (AppUser == null)
                return null;
            return AppUser;
        }
        public async Task<AppUser> Create(AppUser AppUser)
        {
            if (!await AppUserValidator.Create(AppUser))
                return AppUser;

            try
            {
                AppUser.Password = HashPassword(AppUser.Password);
                await UOW.AppUserRepository.Create(AppUser);
                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(AppUser, new { }, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Update(AppUser AppUser)
        {
            if (!await AppUserValidator.Update(AppUser))
                return AppUser;
            try
            {
                var oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                AppUser.Password = oldData.Password;
                AppUser.CreatedAt = oldData.CreatedAt;
                AppUser.DeletedAt = oldData.DeletedAt;

                await UOW.AppUserRepository.Update(AppUser);

                AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
                await Logging.CreateAuditLog(AppUser, oldData, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Delete(AppUser AppUser)
        {
            if (!await AppUserValidator.Delete(AppUser))
                return AppUser;

            try
            {
                await UOW.AppUserRepository.Delete(AppUser);
                await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
                return AppUser;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<List<AppUser>> BulkDelete(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.BulkDelete(AppUsers))
                return AppUsers;

            try
            {
                await UOW.AppUserRepository.BulkDelete(AppUsers);
                await Logging.CreateAuditLog(new { }, AppUsers, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;

        }
        
        public async Task<List<AppUser>> Import(List<AppUser> AppUsers)
        {
            if (!await AppUserValidator.Import(AppUsers))
                return AppUsers;
            try
            {
                await UOW.AppUserRepository.BulkMerge(AppUsers);

                await Logging.CreateAuditLog(AppUsers, new { }, nameof(AppUserService));
                return AppUsers;
            }
            catch (Exception ex)
            {
                await Logging.CreateSystemLog(ex, nameof(AppUserService));
            }
            return null;
        }

        public async Task<AppUser> Login(AppUser AppUser)
        {
            if (!await AppUserValidator.Login(AppUser))
                return AppUser;
            AppUser = await UOW.AppUserRepository.Get(AppUser.Id);
            CurrentContext.UserId = AppUser.Id;
            //await Logging.CreateAuditLog(new { }, AppUser, nameof(AppUserService));
            AppUser.Token = CreateToken(AppUser.Id, AppUser.Username);
            AppUser.RefreshToken = CreateToken(AppUser.Id, AppUser.Username, 43200);

            return AppUser;
        }
        public async Task<AppUser> ChangePassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ChangePassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                oldData.Password = HashPassword(AppUser.NewPassword);
                await UOW.Begin();
                await UOW.AppUserRepository.Update(oldData);
                await UOW.Commit();
                var newData = await UOW.AppUserRepository.Get(AppUser.Id);

                Mail mail = new Mail
                {
                    Subject = "Change Password AppUser",
                    Body = $"Your password has been changed at {StaticParams.DateTimeNow.ToString()}",
                    Recipients = newData.Email,
                    RowId = Guid.NewGuid()
                };
                await MailService.SendMail(mail);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> ForgotPassword(AppUser AppUser)
        {
            if (!await AppUserValidator.ForgotPassword(AppUser))
                return AppUser;
            try
            {
                AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
                {
                    Skip = 0,
                    Take = 1,
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.ALL
                })).FirstOrDefault();

                CurrentContext.UserId = oldData.Id;

                var OtpCode = GenerateOTPCode();
                oldData.OtpCode = HashPassword(OtpCode);
                oldData.OtpExpired = StaticParams.DateTimeNow.AddMinutes(5);

                await UOW.Begin();
                await UOW.AppUserRepository.Update(oldData);
                await UOW.Commit();

                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Otp Code",
                    Body = $"Otp Code recovery password: {OtpCode}",
                    Recipients = newData.Email,
                    RowId = Guid.NewGuid()
                };
                await MailService.SendMail(mail);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> RecoveryPassword(AppUser AppUser)
        {
            if (AppUser.Id == 0)
                return null;
            try
            {
                AppUser oldData = await UOW.AppUserRepository.Get(AppUser.Id);
                CurrentContext.UserId = AppUser.Id;
                oldData.Password = HashPassword(AppUser.Password);
                await UOW.Begin();
                await UOW.AppUserRepository.Update(oldData);
                await UOW.Commit();

                var newData = await UOW.AppUserRepository.Get(oldData.Id);

                Mail mail = new Mail
                {
                    Subject = "Recovery Password",
                    Body = $"Your password has been recovered.",
                    Recipients = newData.Email,
                    RowId = Guid.NewGuid()
                };
                await MailService.SendMail(mail);
                await Logging.CreateAuditLog(newData, oldData, nameof(AppUserService));
                return newData;
            }
            catch (Exception ex)
            {
                await UOW.Rollback();
                if (ex.InnerException == null)
                {
                    await Logging.CreateSystemLog(ex, nameof(AppUserService));
                    throw new MessageException(ex);
                }
                else
                {
                    await Logging.CreateSystemLog(ex.InnerException, nameof(AppUserService));
                    throw new MessageException(ex.InnerException);
                }
            }
        }

        public async Task<AppUser> VerifyOtpCode(AppUser AppUser)
        {
            if (!await AppUserValidator.VerifyOptCode(AppUser))
                return AppUser;
            AppUser appUser = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = AppUser.Email },
                Selects = AppUserSelect.ALL
            })).FirstOrDefault();
            appUser.Token = CreateToken(appUser.Id, appUser.Username, 300);
            return appUser;
        }

        public async Task<AppUser> SaveImage(string path)
        {
            var AppUser = await UOW.AppUserRepository.Get(CurrentContext.UserId);
            AppUser.Avatar = path;
            await UOW.AppUserRepository.Update(AppUser);
            return AppUser;
        }

        public AppUserFilter ToFilter(AppUserFilter filter)
        {
            if (filter.OrFilter == null) filter.OrFilter = new List<AppUserFilter>();
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return filter;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                AppUserFilter subFilter = new AppUserFilter();
                filter.OrFilter.Add(subFilter);
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Id))
                        subFilter.Id = FilterBuilder.Merge(subFilter.Id, FilterPermissionDefinition.IdFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Username))
                        subFilter.Username = FilterBuilder.Merge(subFilter.Username, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Password))
                        subFilter.Password = FilterBuilder.Merge(subFilter.Password, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.DisplayName))
                        subFilter.DisplayName = FilterBuilder.Merge(subFilter.DisplayName, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Email))
                        subFilter.Email = FilterBuilder.Merge(subFilter.Email, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(subFilter.Phone))
                        subFilter.Phone = FilterBuilder.Merge(subFilter.Phone, FilterPermissionDefinition.StringFilter);
                    if (FilterPermissionDefinition.Name == nameof(CurrentContext.UserId) && FilterPermissionDefinition.IdFilter != null)
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.IS.Id)
                        {
                        }
                        if (FilterPermissionDefinition.IdFilter.Equal.HasValue && FilterPermissionDefinition.IdFilter.Equal.Value == CurrentUserEnum.ISNT.Id)
                        {
                        }
                    }
                }
            }
            return filter;
        }

        private string CreateToken(long id, string userName, double? expiredTime = null)
        {
            var secretKey = Configuration["Config:SecretKey"];
            if (expiredTime == null)
                expiredTime = double.TryParse(Configuration["Config:ExpiredTime"], out double time) ? time : 0;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secretKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, id.ToString()),
                    new Claim(ClaimTypes.Name, userName),
                }),
                Expires = StaticParams.DateTimeNow.AddSeconds(expiredTime.Value),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken SecurityToken = tokenHandler.CreateToken(tokenDescriptor);
            string Token = tokenHandler.WriteToken(SecurityToken);
            return Token;
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        private string GeneratePassword()
        {
            const string lower = "abcdefghijklmnopqrstuvwxyz";
            const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string number = "1234567890";
            const string special = "!@#$%^&*_-=+";

            Random _rand = new Random();
            var bytes = new byte[10];
            new RNGCryptoServiceProvider().GetBytes(bytes);

            var res = new StringBuilder();
            foreach (byte b in bytes)
            {
                switch (_rand.Next(4))
                {
                    case 0:
                        res.Append(lower[b % lower.Count()]);
                        break;
                    case 1:
                        res.Append(upper[b % upper.Count()]);
                        break;
                    case 2:
                        res.Append(number[b % number.Count()]);
                        break;
                    case 3:
                        res.Append(special[b % special.Count()]);
                        break;
                }
            }
            return res.ToString();
        }

        private string GenerateOTPCode()
        {
            Random rand = new Random();
            return rand.Next(100000, 999999).ToString();
        }
    }
}
