using System;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class DataField : Attribute
    {
        public string Name;
        public string Control;
        public bool Shared = true;
        public string Group;

        public DataField(string name, string control)
        {
            Name = name;
            Control = control;
        }

    }
}
