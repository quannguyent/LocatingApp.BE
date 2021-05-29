using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Entities;
using LocatingApp;
using LocatingApp.Repositories;
using System.Security.Cryptography;

namespace LocatingApp.Services.MAppUser
{
    public interface IAppUserValidator : IServiceScoped
    {
        Task<bool> Create(AppUser AppUser);
        Task<bool> Update(AppUser AppUser);
        Task<bool> Login(AppUser AppUser);
        Task<bool> ChangePassword(AppUser AppUser);
        Task<bool> ForgotPassword(AppUser AppUser);
        Task<bool> VerifyOptCode(AppUser AppUser);
        Task<bool> Delete(AppUser AppUser);
        Task<bool> BulkDelete(List<AppUser> AppUsers);
        Task<bool> Import(List<AppUser> AppUsers);
    }

    public class AppUserValidator : IAppUserValidator
    {
        public enum ErrorCode
        {
            IdNotExisted,
            PasswordNotMatch,
            UsernameNotExisted,
            EmailNotExisted,
            OtpCodeInvalid,
            OtpExpired,
            UsernameHasSpecialCharacter,
            UsernameOverLength,
            DisplayNameEmpty,
            DisplayNameOverLength,
            UsernameExisted,
            EmailEmpty,
            EmailInvalid,
            EmailOverLength,
            EmailExisted,
            PhoneEmpty,
            PhoneOverLength,
            SexEmpty,
            UsernameEmpty,
        }

        private IUOW UOW;
        private ICurrentContext CurrentContext;

        public AppUserValidator(IUOW UOW, ICurrentContext CurrentContext)
        {
            this.UOW = UOW;
            this.CurrentContext = CurrentContext;
        }

        public async Task<bool> ValidateId(AppUser AppUser)
        {
            AppUserFilter AppUserFilter = new AppUserFilter
            {
                Skip = 0,
                Take = 10,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.Id
            };

            int count = await UOW.AppUserRepository.Count(AppUserFilter);
            if (count == 0)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Id), ErrorCode.IdNotExisted);
            return count == 1;
        }

        public async Task<bool> ValidateUsername(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Username))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameEmpty);
            else
            {
                var Code = AppUser.Username;
                if (AppUser.Username.Contains(" ") || !FilterExtension.ChangeToEnglishChar(Code).Equals(AppUser.Username))
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameHasSpecialCharacter);
                }
                else if (AppUser.Username.Length > 255)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameOverLength);
                }
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = AppUser.Id },
                    Username = new StringFilter { Equal = AppUser.Username },
                    Selects = AppUserSelect.Username
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count != 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameExisted);
            }

            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateDisplayName(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.DisplayName))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameEmpty);
            }
            else if (AppUser.DisplayName.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.DisplayName), ErrorCode.DisplayNameOverLength);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidateEmail(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Email))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailEmpty);
            else if (!IsValidEmail(AppUser.Email))
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailInvalid);
            else
            {
                if (AppUser.Email.Length > 255)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailOverLength);
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Skip = 0,
                    Take = 10,
                    Id = new IdFilter { NotEqual = AppUser.Id },
                    Email = new StringFilter { Equal = AppUser.Email },
                    Selects = AppUserSelect.Email
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count != 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailExisted);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ValidatePhone(AppUser AppUser)
        {
            if (string.IsNullOrEmpty(AppUser.Phone))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneEmpty);
            }
            else if (AppUser.Phone.Length > 255)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Phone), ErrorCode.PhoneOverLength);
            }
            return AppUser.IsValidated;
        }

        private async Task<bool> ValidateSex(AppUser AppUser)
        {
            if (AppUser.SexId != Enums.SexEnum.MALE.Id && AppUser.SexId != Enums.SexEnum.FEMALE.Id)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Sex), ErrorCode.SexEmpty);
            return AppUser.IsValidated;
        }

        public async Task<bool> Create(AppUser AppUser)
        {
            await ValidateUsername(AppUser);
            await ValidateDisplayName(AppUser);
            await ValidateEmail(AppUser);
            await ValidatePhone(AppUser);
            await ValidateSex(AppUser); 
            return AppUser.IsValidated;
        }

        public async Task<bool> Update(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
                await ValidateUsername(AppUser);
                await ValidateDisplayName(AppUser);
                await ValidateEmail(AppUser);
                await ValidatePhone(AppUser);
                await ValidateSex(AppUser);
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> Login(AppUser AppUser)
        {
            if (string.IsNullOrWhiteSpace(AppUser.Username))
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameNotExisted);
                return false;
            }
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Username = new StringFilter { Equal = AppUser.Username },
                Selects = AppUserSelect.ALL,
            });
            if (AppUsers.Count == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.UsernameNotExisted);
            }
            else
            {
                AppUser appUser = AppUsers.FirstOrDefault();
                bool verify = VerifyPassword(appUser.Password, AppUser.Password);
                if (verify == false)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), ErrorCode.PasswordNotMatch);
                }
                else
                {
                    AppUser.Id = appUser.Id;
                }
            }
            return AppUser.IsValidated;
        }

        private bool VerifyPassword(string oldPassword, string newPassword)
        {
            byte[] hashBytes = Convert.FromBase64String(oldPassword);
            /* Get the salt */
            byte[] salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, 16);
            /* Compute the hash on the password the user entered */
            var pbkdf2 = new Rfc2898DeriveBytes(newPassword, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            /* Compare the results */
            for (int i = 0; i < 20; i++)
                if (hashBytes[i + 16] != hash[i])
                    return false;
            return true;
        }

        public async Task<bool> ChangePassword(AppUser AppUser)
        {
            List<AppUser> AppUsers = await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Id = new IdFilter { Equal = AppUser.Id },
                Selects = AppUserSelect.ALL,
            });
            if (AppUsers.Count == 0)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Username), ErrorCode.IdNotExisted);
            }
            else
            {
                AppUser appUser = AppUsers.FirstOrDefault();
                bool verify = VerifyPassword(appUser.Password, AppUser.Password);
                if (verify == false)
                {
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Password), ErrorCode.PasswordNotMatch);
                }
            }
            return AppUser.IsValidated;
        }

        public async Task<bool> ForgotPassword(AppUser AppUser)
        {
            if (AppUser != null && !string.IsNullOrWhiteSpace(AppUser.Email))
            {
                AppUserFilter AppUserFilter = new AppUserFilter
                {
                    Email = new StringFilter { Equal = AppUser.Email },
                };

                int count = await UOW.AppUserRepository.Count(AppUserFilter);
                if (count == 0)
                    AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailNotExisted);
            }

            return AppUser.IsValidated;
        }

        public async Task<bool> VerifyOptCode(AppUser AppUser)
        {
            AppUser oldData = (await UOW.AppUserRepository.List(new AppUserFilter
            {
                Skip = 0,
                Take = 1,
                Email = new StringFilter { Equal = AppUser.Email },
                Selects = AppUserSelect.ALL
            })).FirstOrDefault();
            if (oldData == null)
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.Email), ErrorCode.EmailNotExisted);
            if (oldData.OtpCode != AppUser.OtpCode)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.OtpCode), ErrorCode.OtpCodeInvalid);
            }
            if (DateTime.Now > oldData.OtpExpired)
            {
                AppUser.AddError(nameof(AppUserValidator), nameof(AppUser.OtpExpired), ErrorCode.OtpExpired);
            }

            return AppUser.IsValidated;
        }

        public async Task<bool> Delete(AppUser AppUser)
        {
            if (await ValidateId(AppUser))
            {
            }
            return AppUser.IsValidated;
        }
        
        public async Task<bool> BulkDelete(List<AppUser> AppUsers)
        {
            foreach (AppUser AppUser in AppUsers)
            {
                await Delete(AppUser);
            }
            return AppUsers.All(x => x.IsValidated);
        }
        
        public async Task<bool> Import(List<AppUser> AppUsers)
        {
            return true;
        }

        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
    }
}
