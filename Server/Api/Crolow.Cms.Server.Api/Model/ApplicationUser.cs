namespace Crolow.Cms.Server.Api.Model
{
    using AspNetCore.Identity.MongoDbCore.Models;
    using MongoDB.Bson;
    using MongoDbGenericRepository.Attributes;

    [CollectionName("Users")] // optional: override collection name
    public class ApplicationUser : MongoIdentityUser<ObjectId>
    {
        // Add your extra props here, e.g.:
        // public string DisplayName { get; set; }
    }
}
