using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Crolow.Cms.Server.Managers.Authentication
{
    public static class TokenSecurityKey
    {
        public static SymmetricSecurityKey Create(string secret)
        {
            return new SymmetricSecurityKey(Encoding.ASCII.GetBytes(secret));
        }
    }
}
