
namespace Crolow.Cms.Server.Core.Models.Actions
{
    public enum LoadType
    {
        LoadObject = 1,
        LoadNode = 2,
        LoadRelations = 4,
        LoadTracking = 8,
        LoadTranslations = 16,
        LoadAll = 31,
        LoadObjectTranslated = 17
    }
}
