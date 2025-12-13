using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Model.Templates.Values;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Actions.Messages;
using Crolow.Cms.Server.Core.Models.Databases;
using Crolow.Cms.Server.Core.Models.Templates.Data;
using Crolow.Cms.Server.Core.Models.Templates.Values;
using Crolow.Cms.Server.Managers.Upgrades.Upgrade;
using DataType = Crolow.Cms.Server.Core.Models.Templates.Values.DataType;

namespace Crolow.Cms.Server.Upgrades
{
    [Upgrade(Name = "Crolow.Cms.Core", Version = "0001.0000.0000")]
    public class CoreUpgrade_1_0 : BaseUpgrade, IUpgrade
    {
        public CoreUpgrade_1_0(IModuleProviderManager manager) : base(manager, "Core")
        {
        }

        public void DoUpgrade(BaseRequest request)
        {
            try
            {
                moduleProvider.ClearCache();
                EnsureDataStoreObject<DataStore>(true);
                EnsureDataStoreObject<DataFieldType>(false);
                EnsureDataStoreObject<DataRelationDefinition>(false);
                EnsureDataStoreObject<DataTemplate>(false);
                EnsureDataStoreObject<DataTemplateField>(false);
                EnsureDataStoreObject<DataTemplateSection>(false);
                EnsureDataStoreObject<DataType>(false);
                EnsureDataStoreObject<Editor>(false);
                EnsureDataStoreObject<Transformer>(false);
                EnsureDataStoreObject<Validator>(false);

                DoTemplates(typeof(DataStore).Assembly);
            }
            catch (System.Exception ex)
            {
                request.Response.Responses.Add(ErrorResponse.CreateError<CoreUpgrade_1_0>("Upgrade Error of Core 1.0", 1, ex));
                request.CancelRequest = true;
            }
        }

        public void PostUpgrade(BaseRequest request)
        {
            try
            {
                MoveNewDataStores();
                request.Response.Responses.Add(BaseResponse.Create<CoreUpgrade_1_0>("Successful upgrade to Core 1.0"));
            }
            catch (System.Exception ex)
            {
                request.Response.Responses.Add(ErrorResponse.CreateError<CoreUpgrade_1_0>("Upgrade Error of Core 1.0", 1, ex));
                request.CancelRequest = true;
            }
        }

    }
}