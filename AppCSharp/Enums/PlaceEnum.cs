using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LocatingApp.Enums
{
    public class PlaceEnum
    {
        public static GenericEnum A1 = new GenericEnum { Id = 1, Code = "", Name = "" };
        public static List<GenericEnum> PlaceEnumList = new List<GenericEnum>
        {
            A1,
        };
    }
}
