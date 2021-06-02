using LocatingApp.Common;
using System;
using System.Linq;
using System.Collections.Generic;
using LocatingApp.Entities;

namespace LocatingApp.Rpc.tracking
{
    public class Tracking_AppUserDTO : DataDTO
    {
        
        public long Id { get; set; }
        
        public string Username { get; set; }
        
        public string Password { get; set; }
        
        public string DisplayName { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }
        
        public bool Used { get; set; }
        

        public Tracking_AppUserDTO() {}
        public Tracking_AppUserDTO(AppUser AppUser)
        {
            
            this.Id = AppUser.Id;
            
            this.Username = AppUser.Username;
            
            this.Password = AppUser.Password;
            
            this.DisplayName = AppUser.DisplayName;
            
            this.Email = AppUser.Email;
            
            this.Phone = AppUser.Phone;
            
            this.Errors = AppUser.Errors;
        }
    }

    public class Tracking_AppUserFilterDTO : FilterDTO
    {
        
        public IdFilter Id { get; set; }
        
        public StringFilter Username { get; set; }
        
        public StringFilter Password { get; set; }
        
        public StringFilter DisplayName { get; set; }
        
        public StringFilter Email { get; set; }
        
        public StringFilter Phone { get; set; }
        
        public AppUserOrder OrderBy { get; set; }
    }
}