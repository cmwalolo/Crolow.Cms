using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Model.Templates.Values;
using Crolow.Cms.Server.Core.Models.Data;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Templates.Data
{
    [Template(Schema = "Core", StorageKey = "TemplateFields", NodePath = "templating/fields", NodeName = "Template fields")]
    public class DataTemplateField : DataObject
    {

        public string Name
        {
            get;
            set;
        }


        public IRelationContainer FieldType
        {
            get; set;
        }


        public IRelationContainer Validators
        {
            get; set;
        }


        public string DefaultValue
        {
            get;
            set;
        }


        public bool Storable
        {
            get;
            set;
        }

        [BsonIgnore]
        public IEntityContainer<DataFieldType> DataFieldType { get; set; }

        [BsonIgnore]
        public List<IEntityContainer<Validator>> DataValidators { get; set; }
    }
}
