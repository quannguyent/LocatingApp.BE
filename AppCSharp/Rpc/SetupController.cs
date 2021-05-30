using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Cryptography;
using System.Threading.Tasks;
using LocatingApp.Common;
using LocatingApp.Helpers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using LocatingApp.Entities;
using LocatingApp.Enums;
using LocatingApp.Models;
using RestSharp;

namespace LocatingApp.Rpc
{
    public class SetupController : ControllerBase
    {
        private DataContext DataContext;
        private string AdminPassword;
        private string OrganizationRoot;
        public SetupController(DataContext DataContext, IConfiguration Configuration)
        {
            AdminPassword = Configuration["Config:AdminPassword"];
            OrganizationRoot = Configuration["Config:OrganizationRoot"];
            this.DataContext = DataContext;
        }

        [HttpGet, Route("rpc/portal/ping")]
        public ActionResult Ping()
        {
            return Ok();
        }

        [HttpGet, Route("rpc/portal/setup/init")]
        public ActionResult Init()
        {
            InitEnum();
            //InitAdmin();
            return Ok();
        }

        private ActionResult InitEnum()
        {
            InitStatusEnum();
            InitSexEnum();
            DataContext.SaveChanges();
            return Ok();
        }

        #region permission
        //private ActionResult InitAdmin()
        //{
        //    RoleDAO Admin = DataContext.Role
        //       .Where(r => r.Name == "Admin")
        //       .FirstOrDefault();
        //    if (Admin == null)
        //    {
        //        Admin = new RoleDAO
        //        {
        //            Name = "Admin",
        //            Code = "Admin",
        //        };
        //        DataContext.Role.Add(Admin);
        //        DataContext.SaveChanges();
        //    }

        //    AppUserDAO AppUser = DataContext.AppUser
        //        .Where(au => au.Username.ToLower() == "Administrator".ToLower())
        //        .FirstOrDefault();
        //    if (AppUser == null)
        //    {
        //        OrganizationDAO OrganizationDAO = DataContext.Organization.Where(o => o.Code == OrganizationRoot).FirstOrDefault();
        //        if (OrganizationDAO == null)
        //        {
        //            OrganizationDAO = new OrganizationDAO
        //            {
        //                Address = "",
        //                Code = OrganizationRoot,
        //                Name = OrganizationRoot,
        //                CreatedAt = DateTime.Now,
        //                UpdatedAt = DateTime.Now,
        //                DeletedAt = null,
        //                Email = "",
        //                Level = 1,
        //                ParentId = null,
        //                Phone = "",
        //                RowId = Guid.NewGuid(),
        //                StatusId = 1,
        //                Used = true,
        //                Path = "",
        //            };
        //            DataContext.Organization.Add(OrganizationDAO);
        //            DataContext.SaveChanges();
        //            OrganizationDAO.Path = OrganizationDAO.Id + ".";
        //            DataContext.SaveChanges();
        //        }
        //        AppUser = new AppUserDAO()
        //        {
        //            Username = "Administrator",
        //            Address = "",
        //            Avatar = "",
        //            Birthday = DateTime.Now,
        //            CreatedAt = DateTime.Now,
        //            UpdatedAt = DateTime.Now,
        //            Used = true,
        //            DeletedAt = null,
        //            Department = "",
        //            DisplayName = "Administrator",
        //            Email = "",
        //            OrganizationId = OrganizationDAO.Id,
        //            Password = HashPassword(AdminPassword),
        //            Phone = "",
        //            RowId = Guid.NewGuid(),
        //            SexId = SexEnum.OTHER.Id,
        //            StatusId = 1,
        //        };
        //        DataContext.AppUser.Add(AppUser);
        //        DataContext.SaveChanges();
        //    }

        //    List<SiteDAO> SiteDAOs = DataContext.Site.ToList();
        //    List<AppUserSiteMappingDAO> AppUserSiteMappingDAOs = SiteDAOs.Select(x => new AppUserSiteMappingDAO
        //    {
        //        SiteId = x.Id,
        //        AppUserId = AppUser.Id,
        //        Enabled = true,
        //    }).ToList();
        //    DataContext.AppUserSiteMapping.BulkMerge(AppUserSiteMappingDAOs);

        //    AppUserRoleMappingDAO AppUserRoleMappingDAO = DataContext.AppUserRoleMapping
        //        .Where(ur => ur.RoleId == Admin.Id && ur.AppUserId == AppUser.Id)
        //        .FirstOrDefault();
        //    if (AppUserRoleMappingDAO == null)
        //    {
        //        AppUserRoleMappingDAO = new AppUserRoleMappingDAO
        //        {
        //            AppUserId = AppUser.Id,
        //            RoleId = Admin.Id,
        //        };
        //        DataContext.AppUserRoleMapping.Add(AppUserRoleMappingDAO);
        //        DataContext.SaveChanges();
        //    }

        //    List<MenuDAO> Menus = DataContext.Menu.AsNoTracking()
        //        .Include(v => v.Actions)
        //        .ToList();
        //    List<PermissionDAO> permissions = DataContext.Permission.AsNoTracking()
        //        .Include(p => p.PermissionActionMappings)
        //        .ToList();
        //    foreach (MenuDAO Menu in Menus)
        //    {
        //        PermissionDAO permission = permissions
        //            .Where(p => p.MenuId == Menu.Id && p.RoleId == Admin.Id)
        //            .FirstOrDefault();
        //        if (permission == null)
        //        {
        //            permission = new PermissionDAO
        //            {
        //                Code = Admin + "_" + Menu.Name,
        //                Name = Admin + "_" + Menu.Name,
        //                MenuId = Menu.Id,
        //                RoleId = Admin.Id,
        //                StatusId = StatusEnum.ACTIVE.Id,
        //                PermissionActionMappings = new List<PermissionActionMappingDAO>(),
        //            };
        //            permissions.Add(permission);
        //        }
        //        else
        //        {
        //            permission.StatusId = StatusEnum.ACTIVE.Id;
        //            if (permission.PermissionActionMappings == null)
        //                permission.PermissionActionMappings = new List<PermissionActionMappingDAO>();
        //        }
        //        foreach (ActionDAO action in Menu.Actions)
        //        {
        //            PermissionActionMappingDAO PermissionActionMappingDAO = permission.PermissionActionMappings
        //                .Where(ppm => ppm.ActionId == action.Id).FirstOrDefault();
        //            if (PermissionActionMappingDAO == null)
        //            {
        //                PermissionActionMappingDAO = new PermissionActionMappingDAO
        //                {
        //                    ActionId = action.Id
        //                };
        //                permission.PermissionActionMappings.Add(PermissionActionMappingDAO);
        //            }
        //        }

        //    }
        //    DataContext.Permission.BulkMerge(permissions);
        //    permissions.ForEach(p =>
        //    {
        //        foreach (var action in p.PermissionActionMappings)
        //        {
        //            action.PermissionId = p.Id;
        //        }
        //    });

        //    List<PermissionActionMappingDAO> PermissionActionMappingDAOs = permissions
        //        .SelectMany(p => p.PermissionActionMappings).ToList();
        //    DataContext.PermissionContent.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
        //    DataContext.PermissionActionMapping.Where(pf => pf.Permission.RoleId == Admin.Id).DeleteFromQuery();
        //    DataContext.PermissionActionMapping.BulkMerge(PermissionActionMappingDAOs);
        //    return Ok();
        //}
        #endregion

        private void InitStatusEnum()
        {
            List<StatusDAO> statuses = DataContext.Status.ToList();
            if (!statuses.Any(pt => pt.Id == StatusEnum.ACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.ACTIVE.Id,
                    Code = StatusEnum.ACTIVE.Code,
                    Name = StatusEnum.ACTIVE.Name,
                });
            }

            if (!statuses.Any(pt => pt.Id == StatusEnum.INACTIVE.Id))
            {
                DataContext.Status.Add(new StatusDAO
                {
                    Id = StatusEnum.INACTIVE.Id,
                    Code = StatusEnum.INACTIVE.Code,
                    Name = StatusEnum.INACTIVE.Name,
                });
            }
        }

        private void InitSexEnum()
        {
            List<SexDAO> Sexes = SexEnum.SexEnumList.Select(x => new SexDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.Sex.BulkSynchronize(Sexes);
        }

        private string HashPassword(string password)
        {
            byte[] salt;
            new RNGCryptoServiceProvider().GetBytes(salt = new byte[16]);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            byte[] hash = pbkdf2.GetBytes(20);
            byte[] hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            string savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }
    }
}