using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using Crolow.Cms.Server.Core.Models.Data;
using Crolow.Cms.Server.Core.Models.Templates.Values;
using MongoDB.Bson.Serialization.Attributes;

namespace Crolow.Cms.Server.Core.Models.Templates.Data
{
    [Template(Module = "Core", StorageKey = "DataFieldTypes", NodePath = "Core/Templating/FieldTypes", NodeName = "Template field types")]
    public class DataFieldType : DataObject
    {

        public string Name
        {
            get;
            set;
        }


        public IRelationContainer DataType
        {
            get; set;
        }


        public IRelationContainer Editor
        {
            get; set;
        }


        public IRelationContainer Transformer
        {
            get; set;
        }


        public bool Store
        {
            get;
            set;
        }


        public bool IsArray
        {
            get;
            set;
        }

        [BsonIgnore]
        public IEntityContainer<DataType> DataTypeContainer { get; set; }
        [BsonIgnore]
        public IEntityContainer<Editor> EditorContainer { get; set; }
        [BsonIgnore]
        public IEntityContainer<Transformer> TransformerContainer { get; set; }
    }

}
