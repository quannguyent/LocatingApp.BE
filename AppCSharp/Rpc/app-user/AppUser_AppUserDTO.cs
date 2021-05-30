using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.app_user
{
    public class AppUser_AppUserDTO : DataDTO
    {
        public long Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string OtpCode { get; set; }
        public DateTime OtpExpired { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
        public bool Used { get; set; }
        public long RoleId { get; set; }
        public List<AppUser_LocationLogDTO> LocationLogs { get; set; }
        public Sex Sex { get; set; }
        public Role Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public AppUser_AppUserDTO() {}
        public AppUser_AppUserDTO(AppUser AppUser)
        {
            this.Id = AppUser.Id;
            this.Username = AppUser.Username;
            this.Password = AppUser.Password;
            this.DisplayName = AppUser.DisplayName;
            this.Email = AppUser.Email;
            this.Phone = AppUser.Phone;
            this.Used = AppUser.Used;
            this.SexId = AppUser.SexId;
            this.Birthday = AppUser.Birthday;
            this.RoleId = AppUser.RoleId;
            this.LocationLogs = AppUser.LocationLogs?.Select(x => new AppUser_LocationLogDTO(x)).ToList();
            this.Sex = AppUser.Sex;
            this.Role = AppUser.Role;
            this.CreatedAt = AppUser.CreatedAt;
            this.UpdatedAt = AppUser.UpdatedAt;
            this.Errors = AppUser.Errors;
        }
    }

    public class AppUser_AppUserFilterDTO : FilterDTO
    {
        public IdFilter Id { get; set; }
        public StringFilter Username { get; set; }
        public StringFilter Password { get; set; }
        public StringFilter DisplayName { get; set; }
        public StringFilter Email { get; set; }
        public StringFilter Phone { get; set; }
        public IdFilter RoleId { get; set; }
        public IdFilter SexId { get; set; }
        public DateFilter Birthday { get; set; }
        public DateFilter CreatedAt { get; set; }
        public DateFilter UpdatedAt { get; set; }
        public AppUserOrder OrderBy { get; set; }
    }
}
