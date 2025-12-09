using Crolow.Cms.Server.Core.Models.Templates.Data;
using MongoDB.Bson;
using System;

namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface ITemplateProvider
    {
        DataTemplate GetTemplate(ObjectId link);
        DataTemplate GetTemplate(Type t);
    }
}