using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class SexEnum
    {
        public static GenericEnum MALE => new GenericEnum { Id = 1, Name = "Nam", Code = "Male" };
        public static GenericEnum FEMALE => new GenericEnum { Id = 2, Name = "Nữ", Code = "Female" };
        public static GenericEnum OTHER => new GenericEnum { Id = 3, Name = "Khác", Code = "Other" };
        public static List<GenericEnum> SexEnumList = new List<GenericEnum>
        {
            MALE, FEMALE, OTHER,
        };
    }
}
