using System.Threading.Tasks;

namespace Crolow.Cms.Server.Api.Services
{
    public interface ISmsSender
    {
        Task SendSmsAsync(string number, string message);


    }
}

