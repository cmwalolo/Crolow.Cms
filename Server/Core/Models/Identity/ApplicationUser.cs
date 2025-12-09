using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Crolow.Cms.Server.Core.Models.Identity
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    [Common.Attributes.Template(Schema = "Core", StorageKey = "Users")]
    public class ApplicationUser : IdentityUser
    {
    }
}
