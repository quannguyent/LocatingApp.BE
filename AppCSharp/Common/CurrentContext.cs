using System.Collections.Generic;

namespace LocatingApp.Common
{
    public interface ICurrentContext : IServiceScoped
    {
        long UserId { get; set; }
        string UserName { get; set; }
        int TimeZone { get; set; }
        string Language { get; set; }
        string Token { get; set; }
        long RoleId { get; set; }
        Dictionary<long, List<FilterPermissionDefinition>> Filters { get; set; }
    }

    public class CurrentContext : ICurrentContext
    {
        public long UserId { get; set; }
        public string UserName { get; set; }
        public int TimeZone { get; set; }
        public string Language { get; set; } = "vi";
        public string Token { get; set; }
        public long RoleId { get; set; }
        public Dictionary<long, List<FilterPermissionDefinition>> Filters { get; set; }
    }
}
