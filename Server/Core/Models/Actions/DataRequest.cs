using MongoDB.Bson;


namespace Crolow.Cms.Server.Core.Models.Actions
{
    public class DataRequest : BaseRequest

    {
        public DataRequest()
        {
            LoadType = LoadType.LoadAll;
        }
        public DataRequest(LoadType loadType)
        {
            LoadType = loadType;
        }

        public LoadType LoadType { get; set; }

        public ObjectId DataLink { get; set; }
        public string Language
        {
            get; set;
        }
    }
}
