using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Crolow.Cms.Server.Common.Interfaces
{
    public interface IApplicationStartup
    {
        void ConfigureServices(IServiceCollection services, IConfiguration config);
        void Configure(IApplicationBuilder app, IWebHostEnvironment env);
    }
}