using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class CheckingStatusEnum
    {
        public static GenericEnum In = new GenericEnum { Id = 1, Code = "In", Name = "In" };
        public static GenericEnum Out = new GenericEnum { Id = 2, Code = "In", Name = "Out" };
        public static List<GenericEnum> CheckingStatusEnumList = new List<GenericEnum>
        {
            In, Out
        };
    }
}
