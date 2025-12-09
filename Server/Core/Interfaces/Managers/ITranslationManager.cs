using Crolow.Cms.Server.Core.Interfaces.Models.Data;
using MongoDB.Bson;
using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface ITranslationManager
    {
        IEnumerable<IDataTranslation> GetAllTranslations(ObjectId link);
        IEnumerable<IDataTranslation> GetAllTranslations(IDataObject dataObject);
        IEnumerable<IDataTranslation> GetTranslations(ObjectId link, string language);
        IEnumerable<IDataTranslation> GetTranslations(IDataObject dataObject, string language);
        void Update(IDataTranslation node);
        void Update(List<IDataTranslation> node);
    }
}