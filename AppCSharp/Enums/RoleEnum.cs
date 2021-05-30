using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class RoleEnum
    {
        public static GenericEnum User => new GenericEnum { Id = 1, Name = "User", Code = "User" };
        public static GenericEnum Admin => new GenericEnum { Id = 2, Name = "Admin", Code = "Admin" };
        public static GenericEnum Analysis => new GenericEnum { Id = 3, Name = "Analysis", Code = "Analysis" };
        public static List<GenericEnum> RoleEnumList = new List<GenericEnum>
        {
            User, Admin, Analysis
        };
    }
}
