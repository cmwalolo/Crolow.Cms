using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ComponentAttribute : Attribute
    {
        public string Path { get; set; }
        public ComponentAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Class)]
    public class ParserAttribute : Attribute
    {
        public Type Type { get; set; }
        public ParserAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
