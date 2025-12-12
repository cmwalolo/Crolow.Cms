using Crolow.Cms.Server.Common.Configuration;
using Crolow.Cms.Server.Core.Models.Configuration;
using Crolow.Cms.Server.Managers.Authentication;
using Crolow.Cms.Server.Managers.Startup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Threading.Tasks;

namespace Kalow.Apps.Api
{
    using Crolow.Cms.Server.Api.Model;
    using Microsoft.Extensions.Hosting;
    using MongoDB.Bson;
    using System;

    public class Startup
    {
        public Startup(IWebHostEnvironment env)   // <- IWebHostEnvironment for .NET 8+
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile("appsettings.upgrades.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            this.Configuration = builder.Build();
        }

        public IConfiguration Configuration { get; }

        public virtual void ConfigureServices(IServiceCollection services)
        {
            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                    builder
                        .WithOrigins(Configuration.GetValue<string>("AllowedOrigins").Split(','))
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });

            services.AddHttpClient();

            // ✅ Full MVC: Controllers + Views + JSON (Newtonsoft)
            services
                .AddControllersWithViews()
                .AddNewtonsoftJson();

            // Swagger
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
                c.EnableAnnotations();
            });

            // Auth + Identity
            ConfigureAuthorization(services);

            // Options + other services
            ConfigureOptions(services);
            StartupManager.ConfigureServices(services, Configuration);
        }

        // Configure HTTP pipeline
        public virtual void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILoggerFactory loggerFactory)
        {
            // Logging
            loggerFactory.AddFile("Logs/myapp-{Date}.txt");

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();

            // Static files (wwwroot)
            app.UseStaticFiles(new StaticFileOptions { ServeUnknownFileTypes = true });

            app.UseRouting();

            // CORS should be between UseRouting and UseAuthorization
            app.UseCors("CorsPolicy");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                // API controllers (attribute-routed)
                endpoints.MapControllers();

                // MVC views (default route for classic controllers with views)
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                // If you ever use Razor Pages:
                // endpoints.MapRazorPages();
            });

            // Any extra app-specific startup
            StartupManager.Configure(app, env);
        }

        #region Private Methods

        private void ConfigureOptions(IServiceCollection services)
        {
            services.Configure<DatabaseSettings>(options =>
            {
                Configuration.GetSection("DatabaseSettings").Bind(options);
            });

            services.ConfigureWritable<UpgradeSettings>(
                Configuration.GetSection("Upgrades"),
                "appsettings.upgrades.json");
        }

        public class MongoDbSettings
        {
            public string ConnectionString { get; set; }
            public string DatabaseName { get; set; }
        }

        private void ConfigureAuthorization(IServiceCollection services)
        {
            services
                .AddIdentity<ApplicationUser, ApplicationRole>(options =>
                {
                    options.Password.RequireDigit = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                })
                .AddMongoDbStores<ApplicationUser, ApplicationRole, ObjectId>(
                    Configuration["MongoIdentity:ConnectionString"],
                    Configuration["MongoIdentity:DatabaseName"])
                .AddSignInManager()
                .AddDefaultTokenProviders();

            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = false,
                        ValidateIssuerSigningKey = false,

                        ValidIssuer = "Kalow.Apps.Bearer",
                        ValidAudience = "Kalow.Apps.Bearer",
                        ClockSkew = TimeSpan.FromMinutes(5),
                        IssuerSigningKey = TokenSecurityKey.Create(Configuration["JWTToken"])
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnMessageReceived = context =>
                        {
                            var accessToken = context.Request.Query["access_token"];

                            var path = context.HttpContext.Request.Path;
                            if (!string.IsNullOrEmpty(accessToken) &&
                                path.StartsWithSegments("/topmachine/hypergram/hub"))
                            {
                                context.Token = accessToken;
                            }
                            return Task.CompletedTask;
                        }
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("admin", policy =>
                {
                    policy.AuthenticationSchemes.Add(JwtBearerDefaults.AuthenticationScheme);
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("admin");
                });
            });
        }

        #endregion
    }
}
