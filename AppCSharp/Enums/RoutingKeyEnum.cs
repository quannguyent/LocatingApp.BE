using LocatingApp.Common;
using System.Collections.Generic;

namespace LocatingApp.Enums
{
    public class RoutingKeyEnum
    {
        public static GenericEnum AuditLogSend = new GenericEnum { Id = 5, Code = "AuditLog.Send", Name = "Audit Log" };
        public static GenericEnum SystemLogSend = new GenericEnum { Id = 6, Code = "SystemLog.Send", Name = "System Log" };
        public static List<GenericEnum> RoutingKeyEnumList = new List<GenericEnum>()
        {
            AuditLogSend, SystemLogSend
        };
    }
}
