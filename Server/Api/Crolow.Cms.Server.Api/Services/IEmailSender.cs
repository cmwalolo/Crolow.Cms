using System.Threading.Tasks;

namespace Crolow.Cms.Server.Api.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
