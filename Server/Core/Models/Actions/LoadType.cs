
namespace Crolow.Cms.Server.Core.Models.Actions
{
    public enum LoadType
    {
        LoadObject = 1,
        LoadNode = 2,
        LoadRelations = 4,
        LoadTranslations = 8,
        LoadAll = 255,
        LoadObjectTranslated = 9
    }
}
