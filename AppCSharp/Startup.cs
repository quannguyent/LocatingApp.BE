using LocatingApp.Common;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Primitives;
using Microsoft.Extensions.ObjectPool;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Hangfire;
using Hangfire.SqlServer;
using Hangfire.Storage;
using OfficeOpenXml;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Z.EntityFramework.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using LocatingApp.Models;
using LocatingApp.Rpc;
//using LocatingApp.Handlers;
using LocatingApp.Services;

namespace LocatingApp
{
    public class Startup
    {
        public Startup(IHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
            .SetBasePath(env.ContentRootPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

            Configuration = builder.Build();
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
            LicenseManager.AddLicense("2456;100-FPT", "3f0586d1-0216-5005-8b7a-9080b0bedb5e");
            string licenseErrorMessage;
            if (!LicenseManager.ValidateLicense(out licenseErrorMessage))
                throw new Exception(licenseErrorMessage);
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            _ = DataEntity.ErrorResource;
            services.AddControllers().AddNewtonsoftJson();
            services.AddSingleton<ObjectPoolProvider, DefaultObjectPoolProvider>();
            //services.AddSingleton<IPooledObjectPolicy<IModel>, RabbitModelPooledObjectPolicy>();
            //services.AddSingleton<IRabbitManager, RabbitManager>();
            //services.AddHostedService<ConsumeRabbitMQHostedService>();

            services.AddDbContext<DataContext>(options =>
               options.UseSqlServer(Configuration.GetConnectionString("DataContext")));
            EntityFrameworkManager.ContextFactory = context =>
            {
                var optionsBuilder = new DbContextOptionsBuilder<DataContext>();
                optionsBuilder.UseSqlServer(Configuration.GetConnectionString("DataContext"));
                DataContext DataContext = new DataContext(optionsBuilder.Options);
                return DataContext;
            };

            services.Scan(scan => scan
             .FromAssemblyOf<IServiceScoped>()
                 .AddClasses(classes => classes.AssignableTo<IServiceScoped>())
                     .AsImplementedInterfaces()
                     .WithScopedLifetime());
            
            services.AddHangfire(configuration => configuration
             .SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
             .UseSimpleAssemblyNameTypeSerializer()
             .UseRecommendedSerializerSettings()
             .UseSqlServerStorage(Configuration.GetConnectionString("DataContext"), new SqlServerStorageOptions
             {
                 SlidingInvisibilityTimeout = TimeSpan.FromMinutes(2),
                 QueuePollInterval = TimeSpan.FromSeconds(10),
                 CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                 UseRecommendedIsolationLevel = true,
                 UsePageLocksOnDequeue = true,
                 DisableGlobalLocks = true
             }));
            services.AddHangfireServer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.Events = new JwtBearerEvents
                 {
                     OnMessageReceived = context =>
                     {
                         context.Token = context.Request.Cookies["Token"];
                         return Task.CompletedTask;
                     }
                 };
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKeyResolver = (token, secutiryToken, kid, validationParameters) =>
                    {
                        var secretKey = Configuration["Config:SecretKey"];
                        var key = Encoding.ASCII.GetBytes(secretKey);
                        SecurityKey issuerSigningKey = new SymmetricSecurityKey(key);
                        return new List<SecurityKey>() { issuerSigningKey };
                    }
                };
            });

            services.AddTransient<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthorizationHandler, PermissionHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permission", policy =>
                    policy.Requirements.Add(new PermissionRequirement()));
            });

            services.AddScoped<IAuthorizationHandler, SimpleHandler>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Simple", policy =>
                    policy.Requirements.Add(new SimpleRequirement()));
            });

            Action onChange = () =>
            {
                JobStorage.Current = new SqlServerStorage(Configuration.GetConnectionString("DataContext"));
                using (var connection = JobStorage.Current.GetConnection())
                {
                    foreach (var recurringJob in connection.GetRecurringJobs())
                    {
                        RecurringJob.RemoveIfExists(recurringJob.Id);
                    }
                }

                bool isWindows = System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows);
                TimeZoneInfo tzi;
                if (isWindows)
                    tzi = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");
                else
                    tzi = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");
                RecurringJob.AddOrUpdate<MaintenanceService>("CleanHangfire", x => x.CleanHangfire(), Cron.Daily, tzi);
            };
            onChange();
            ChangeToken.OnChange(() => Configuration.GetReloadToken(), onChange);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseRouting();
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            app.UseSwagger(c =>
            {
                c.RouteTemplate = "rpc/locatingapp/swagger/{documentname}/swagger.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/rpc/locatingapp/swagger/v1/swagger.json", "locatingapp API");
                c.RoutePrefix = "rpc/locatingapp/swagger";
            });
            app.UseHangfireDashboard("/rpc/dms/hangfire");
            
        }
    }
}
