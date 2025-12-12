using Kalow.Apps.Managers.Upgrades.Upgrade;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Kalow.Apps.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // We make sure some DLLs are loaded 
            // as our reflection based configuration is not discovering some classes 
            // TO be reviewed !
            CoreUpgrade_1_0 pp = new CoreUpgrade_1_0(null);
            //TopMachine_1_0 tt = new TopMachine_1_0(null);
            //var hgu = new Kalow.Hypergram.Upgrades.Hypergram_1_0(null);

            //var cs = new Topmachine.Topping.Api.Startup.CoreStartup();
            //var dcs = new Crolow.OpenAI.Dictionary.CoreStartup();
            //var hcs = new Hypergram.Api.Startup.CoreStartup();


            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .Build();
    }
}
