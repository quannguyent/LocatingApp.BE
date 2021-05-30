using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.app_user
{
    public class AppUser_SignupDTO : DataDTO
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string DisplayName { get; set; }
        public long SexId { get; set; }
        public DateTime Birthday { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public AppUser_SignupDTO() {}
        public AppUser_SignupDTO(AppUser AppUser)
        {
            this.Username = AppUser.Username;
            this.Password = AppUser.Password;
            this.DisplayName = AppUser.DisplayName;
            this.Email = AppUser.Email;
            this.Phone = AppUser.Phone;
            this.Errors = AppUser.Errors;
        }
    }
}
