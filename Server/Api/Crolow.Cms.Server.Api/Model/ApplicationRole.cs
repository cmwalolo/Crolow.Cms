namespace Crolow.Cms.Server.Api.Model
{
    using AspNetCore.Identity.MongoDbCore.Models;
    using MongoDB.Bson;
    using MongoDbGenericRepository.Attributes;

    [CollectionName("Roles")]
    public class ApplicationRole : MongoIdentityRole<ObjectId>
    {
    }
}
