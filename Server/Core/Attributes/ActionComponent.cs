using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class ActionComponentAttribute : Attribute
    {
        public string Path { get; set; }
        public string ExecutionOrder { get; set; }
        public ActionComponentAttribute()
        {
        }
    }

}
