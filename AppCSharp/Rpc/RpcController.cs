using LocatingApp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Reflection;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace LocatingApp.Rpc
{
    [Authorize]
    [Authorize(Policy ="Permission")]
    public class RpcController : ControllerBase
    {
    }

    [Authorize]
    [Authorize(Policy = "Simple")]
    public class SimpleController : ControllerBase
    {
    }

    public class PermissionRequirement : IAuthorizationRequirement
    {
        public PermissionRequirement()
        {
        }
    }

    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        private ICurrentContext CurrentContext;
        //private DataContext DataContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        public PermissionHandler(
            ICurrentContext CurrentContext, 
            //DataContext DataContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            this.CurrentContext = CurrentContext;
            //this.DataContext = DataContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, PermissionRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var HttpContext = httpContextAccessor.HttpContext;
            string url = HttpContext.Request.Path.Value.ToLower().Substring(1);
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            CurrentContext.Token = HttpContext.Request.Cookies["Token"];
            CurrentContext.UserId = UserId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";
            context.Succeed(requirement);
            //List<long> permissionIds = await DataContext.AppUserPermission
            //    .Where(p => p.AppUserId == UserId && p.Path == url)
            //    .Select(p => p.PermissionId).ToListAsync();

            //List<long> permissionIds = await DataContext.AppUserPermission
            //    .Where(p => p.AppUserId == UserId && p.Path == url)
            //    .Select(p => p.PermissionId).ToListAsync();

            //if (permissionIds.Count == 0)
            //{
            //    context.Fail();
            //    return;
            //}
            //List<PermissionDAO> PermissionDAOs = await DataContext.Permission.AsNoTracking()
            //    .Include(p => p.PermissionContents).ThenInclude(pf => pf.Field)
            //    .Where(p => permissionIds.Contains(p.Id))
            //    .ToListAsync();
            //CurrentContext.RoleIds = PermissionDAOs.Select(p => p.RoleId).Distinct().ToList();
            //CurrentContext.Filters = new Dictionary<long, List<FilterPermissionDefinition>>();
            //foreach (PermissionDAO PermissionDAO in PermissionDAOs)
            //{
            //    List<FilterPermissionDefinition> FilterPermissionDefinitions = new List<FilterPermissionDefinition>();
            //    CurrentContext.Filters.Add(PermissionDAO.Id, FilterPermissionDefinitions);
            //    foreach (PermissionContentDAO PermissionContentDAO in PermissionDAO.PermissionContents)
            //    {
            //        FilterPermissionDefinition FilterPermissionDefinition = FilterPermissionDefinitions.Where(f => f.Name == PermissionContentDAO.Field.Name).FirstOrDefault();
            //        if (FilterPermissionDefinition == null)
            //        {
            //            FilterPermissionDefinition = new FilterPermissionDefinition(PermissionContentDAO.Field.Name, PermissionContentDAO.Field.FieldTypeId, PermissionContentDAO.PermissionOperatorId, PermissionContentDAO.Value);
            //            FilterPermissionDefinitions.Add(FilterPermissionDefinition);
            //        }
            //    }

            //}
            context.Succeed(requirement);
        }
       
    }

    public class SimpleRequirement : IAuthorizationRequirement
    {
        public SimpleRequirement()
        {
        }
    }
    public class SimpleHandler : AuthorizationHandler<SimpleRequirement>
    {
        private ICurrentContext CurrentContext;
        private readonly IHttpContextAccessor httpContextAccessor;
        public SimpleHandler(ICurrentContext CurrentContext, IHttpContextAccessor httpContextAccessor)
        {
            this.CurrentContext = CurrentContext;
            this.httpContextAccessor = httpContextAccessor;
        }
        protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, SimpleRequirement requirement)
        {
            var types = context.User.Claims.Select(c => c.Type).ToList();
            if (!context.User.HasClaim(c => c.Type == ClaimTypes.NameIdentifier))
            {
                context.Fail();
                return;
            }
            long UserId = long.TryParse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value, out long u) ? u : 0;
            string UserName = context.User.FindFirst(c => c.Type == ClaimTypes.Name).Value;
            var HttpContext = httpContextAccessor.HttpContext;
            string url = HttpContext.Request.Path.Value.ToLower().Substring(1);
            string TimeZone = HttpContext.Request.Headers["X-TimeZone"];
            string Language = HttpContext.Request.Headers["X-Language"];
            CurrentContext.Token = HttpContext.Request.Cookies["Token"];
            CurrentContext.UserId = UserId;
            CurrentContext.TimeZone = int.TryParse(TimeZone, out int t) ? t : 0;
            CurrentContext.Language = Language ?? "vi";
            context.Succeed(requirement);
        }
    }
}
