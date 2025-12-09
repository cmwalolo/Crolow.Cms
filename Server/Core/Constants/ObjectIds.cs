using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Constants
{
    public enum ObjectIdStamps
    {
        // Stores 
        Datastore = 1,
        TemplateDatastore = 2,
        NodeDatastore = 3,
        RelationDatastore = 4,
        TranslationDatastore = 5,

        //Templates 
        DataStoreTemplate = 20,
        TemplateTemplate = 21,
    }

    public static class ObjectIds
    {
        public static readonly ObjectId DataStoreId = CreateObjectId(ObjectIdStamps.Datastore);
        public static readonly ObjectId TemplateDataStoreId = CreateObjectId(ObjectIdStamps.TemplateDatastore);

        public static readonly ObjectId DataStoreTemplateId = CreateObjectId(ObjectIdStamps.DataStoreTemplate);
        public static readonly ObjectId TemplateTemplateId = CreateObjectId(ObjectIdStamps.TemplateTemplate);

        public static ObjectId CreateObjectId(int stamp)
        {
            return ObjectId.GenerateNewId(stamp);
        }

        public static ObjectId CreateObjectId(ObjectIdStamps stamp)
        {
            return ObjectId.GenerateNewId((int)stamp);
        }
    }
}