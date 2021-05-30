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
            InitAdmin();
            return Ok();
        }

        private ActionResult InitEnum()
        {
            InitStatusEnum();
            InitSexEnum();
            InitRoleEnum();
            DataContext.SaveChanges();
            return Ok();
        }

        #region permission
        private ActionResult InitAdmin()
        {
            RoleDAO Admin = DataContext.Role
               .Where(r => r.Name == "Admin")
               .FirstOrDefault();
            if (Admin == null)
            {
                Admin = new RoleDAO
                {
                    Name = "Admin",
                    Code = "Admin",
                };
                DataContext.Role.Add(Admin);
                DataContext.SaveChanges();
            }

            AppUserDAO AppUser = DataContext.AppUser
                .Where(au => au.Username.ToLower() == "Administrator".ToLower())
                .FirstOrDefault();
            if (AppUser == null)
            {
                AppUser = new AppUserDAO()
                {
                    Username = "Administrator",
                    Birthday = DateTime.Now,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Used = true,
                    DeletedAt = null,
                    DisplayName = "Administrator",
                    Email = "",
                    Password = HashPassword(AdminPassword),
                    Phone = "",
                    SexId = SexEnum.OTHER.Id,
                    RoleId = RoleEnum.Admin.Id
                };
                DataContext.AppUser.Add(AppUser);
                DataContext.SaveChanges();
            }
            return Ok();
        }
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

        private void InitRoleEnum()
        {
            List<RoleDAO> Rolees = RoleEnum.RoleEnumList.Select(x => new RoleDAO
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
            }).ToList();
            DataContext.Role.BulkSynchronize(Rolees);
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