using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Rpc.app_user
{
    public class AppUser_ChangePasswordDTO
    {
        public long Id { get; set; }
        public string NewPassword { get; set; }
    }
}
