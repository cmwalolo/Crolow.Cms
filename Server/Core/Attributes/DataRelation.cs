using System;
//using Crolow.Cms.Server.Core.Enumerations;

namespace Crolow.Cms.Server.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class DataRelationAttribute : Attribute
    {
       // public DataRelationType RelationType { get; set; }
      //  public DataRelationTarget RelationTarget { get; set; }
      //  public DataRelationTarget DataRelationCount { get; set; }

        public DataRelationAttribute()
        {
        }
    }
}
