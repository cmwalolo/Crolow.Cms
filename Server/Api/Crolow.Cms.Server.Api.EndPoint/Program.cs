using Crolow.Cms.Server.Upgrades;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Kalow.Apps.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // *** TODO
            // We make sure the upgrade assembly is loaded
            // Should be changed by something more elegant in the future

            var tt = typeof(CoreUpgrade_1_0);

            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
