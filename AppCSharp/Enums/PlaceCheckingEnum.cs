using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class PlaceCheckingEnum
    {
        public static GenericEnum A1 = new GenericEnum { Id = 1, Code = "", Name = "" };
        public static List<GenericEnum> PlaceCheckingEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
