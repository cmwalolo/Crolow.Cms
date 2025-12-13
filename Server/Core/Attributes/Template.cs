using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateAttribute : Attribute
    {
        public string Database;
        public string Module;
        public string StorageKey;
        public bool System;
        public string NodeName;
        public string NodePath;
        public TemplateAttribute()
        {
            Database = "CrolowCms-Core";
            Module = "Core";
        }
    }
}
