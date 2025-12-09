using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Templates.Data
{
    [Template(Schema = "Core", StorageKey = "TemplateSections", NodePath = "templating/sections", NodeName = "Template Sections")]
    public class DataTemplateSection : DataObject
    {


        public string Prefix
        {
            get;
            set;
        }


        public string Name
        {
            get;
            set;
        }

        [BsonIgnore]
        public List<IEntityContainer<DataTemplateField>> Fields { get; set; }


    }
}
