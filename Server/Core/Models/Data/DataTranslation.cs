using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Enums;
using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Kalow.Apps.Models.Data
{
    [Template(Module = "Core", StorageKey = "Translations")]
    public class DataTranslation : IDataTranslation
    {
        public DataTranslation()
        {
            Id = ObjectId.GenerateNewId();
        }

        public ObjectId Id { get; set; }
        public string Language { get; set; }
        public object Translation { get; set; }
        [BsonIgnore]
        public EditState EditState { get; set; }
    }
}
