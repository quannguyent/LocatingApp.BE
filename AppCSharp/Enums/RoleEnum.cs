using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class RoleEnum
    {
        public static GenericEnum User => new GenericEnum { Id = 1, Name = "User", Code = "USER" };
        public static GenericEnum Admin => new GenericEnum { Id = 2, Name = "Admin", Code = "ADMIN" };
        public static GenericEnum Analysis => new GenericEnum { Id = 3, Name = "Analysis", Code = "ANALYSIS" };
        public static List<GenericEnum> SexEnumList = new List<GenericEnum>
        {
            User, Admin, Analysis
        };
    }
}
