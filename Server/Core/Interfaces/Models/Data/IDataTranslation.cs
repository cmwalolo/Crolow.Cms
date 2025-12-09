using Crolow.Cms.Server.Core.Enums;
using MongoDB.Bson;

namespace Crolow.Cms.Server.Core.Interfaces.Models.Data
{
    public interface IDataTranslation
    {
        string Language { get; set; }
        ObjectId Id { get; set; }
        object Translation { get; set; }
        EditState EditState { get; set; }

    }
}