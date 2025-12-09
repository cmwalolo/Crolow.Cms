using MongoDB.Bson;

namespace Kalow.Apps.Managers.Providers
{
    public class IdProvider// : IBsonProvider
    {
        public string GetId()
        {
            return ObjectId.GenerateNewId().ToString();
        }
    }
}
