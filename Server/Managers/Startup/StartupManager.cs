using Crolow.Apps.Common.Reflection;
using Crolow.Cms.Server.Common.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kalow.Apps.Managers.Startup
{
    public class StartupManager
    {
        public static void ConfigureServices(IServiceCollection services, IConfiguration config)
        {
            var startups = ReflectionHelper.GetSubclassesOf(typeof(IApplicationStartup), true);

            foreach (var startup in startups)
            {
                ((IApplicationStartup)Activator.CreateInstance(startup)).ConfigureServices(services, config);
            }
        }

        public static void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            var startups = ReflectionHelper.GetSubclassesOf(typeof(IApplicationStartup), true);

            foreach (var startup in startups)
            {
                ((IApplicationStartup)Activator.CreateInstance(startup)).Configure(app, env);
            }

        }
    }
}
