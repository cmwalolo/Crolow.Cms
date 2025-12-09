using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UpgradeAttribute : Attribute
    {
        public string Version;
        public string Name;
    }
}
