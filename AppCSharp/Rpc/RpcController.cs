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
using LocatingApp.Services.MAppUser;
using LocatingApp.Entities;

namespace LocatingApp.Rpc
{
    [Authorize]
    [Authorize(Policy = "Permission")]
    public class RpcController : ControllerBase
    {
        protected async Task<List<long>> FilterFriend(IAppUserService AppUserService, ICurrentContext CurrentContext)
        {
            if (CurrentContext.Filters == null || CurrentContext.Filters.Count == 0) return new List<long>();
            List<AppUser> AppUsers = await AppUserService.List(new AppUser
            {
                Skip = 0,
                Take = int.MaxValue,
                Selects = OrganizationSelect.ALL,
                OrderBy = OrganizationOrder.Id,
                OrderType = OrderType.ASC
            });

            List<long> In = null;
            List<long> NotIn = null;
            foreach (var currentFilter in CurrentContext.Filters)
            {
                List<FilterPermissionDefinition> FilterPermissionDefinitions = currentFilter.Value;
                foreach (FilterPermissionDefinition FilterPermissionDefinition in FilterPermissionDefinitions)
                {
                    if (FilterPermissionDefinition.Name == "OrganizationId")
                    {
                        if (FilterPermissionDefinition.IdFilter.Equal != null)
                        {
                            if (In == null) In = new List<long>();
                            In.Add(FilterPermissionDefinition.IdFilter.Equal.Value);
                        }
                        if (FilterPermissionDefinition.IdFilter.In != null)
                        {
                            if (In == null) In = new List<long>();
                            In.AddRange(FilterPermissionDefinition.IdFilter.In);
                        }

                        if (FilterPermissionDefinition.IdFilter.NotEqual != null)
                        {
                            if (NotIn == null) NotIn = new List<long>();
                            NotIn.Add(FilterPermissionDefinition.IdFilter.NotEqual.Value);
                        }
                        if (FilterPermissionDefinition.IdFilter.NotIn != null)
                        {
                            if (NotIn == null) NotIn = new List<long>();
                            NotIn.AddRange(FilterPermissionDefinition.IdFilter.NotIn);
                        }
                    }
                }
            }

            if (In != null)
            {
                List<string> InPaths = Organizations.Where(o => In.Count == 0 || In.Contains(o.Id)).Select(o => o.Path).ToList();
                Organizations = Organizations.Where(o => InPaths.Any(p => o.Path.StartsWith(p))).ToList();
            }
            if (NotIn != null)
            {
                List<string> NotInPaths = Organizations.Where(o => NotIn.Count == 0 || NotIn.Contains(o.Id)).Select(o => o.Path).ToList();
                Organizations = Organizations.Where(o => !NotInPaths.Any(p => o.Path.StartsWith(p))).ToList();
            }

            List<long> organizationIds = Organizations.Select(o => o.Id).ToList();

            return organizationIds;
        }
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
