using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class TemplateAttribute : Attribute
    {
        public string Database;
        public string Schema;
        public string StorageKey;
        public bool System;
        public string NodeName;
        public string NodePath;
        public TemplateAttribute()
        {
            Database = "Core";
            Schema = "Core";
        }
    }
}
