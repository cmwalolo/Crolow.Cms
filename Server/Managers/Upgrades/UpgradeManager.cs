using Crolow.Apps.Common.Reflection;
using Crolow.Cms.Server.Common.Configuration;
using Crolow.Cms.Server.Core.Attributes;
using Crolow.Cms.Server.Core.Interfaces.Managers;
using Crolow.Cms.Server.Core.Models.Actions;
using Crolow.Cms.Server.Core.Models.Configuration;

namespace Crolow.Cms.Server.Managers.Upgrades
{
    public class UpgradeManager : IUpgradeManager
    {
        protected readonly IManagerFactory managerFactory;
        protected readonly IWritableOptions<UpgradeSettings> upgradeSettings;

        protected IDatabaseProvider databaseProvider => managerFactory.DatabaseProvider;
        protected INodeManager nodeManager => managerFactory.NodeManager;

        public UpgradeManager(IWritableOptions<UpgradeSettings> upgradeSettings,
                        IManagerFactory managerFactory
            )
        {
            this.upgradeSettings = upgradeSettings;
            this.managerFactory = managerFactory;
        }

        public void DoUpgrade(BaseRequest request)
        {
            var upgraders = ReflectionHelper.GetClassesWithAttribute(typeof(UpgradeAttribute), true)
                .ToDictionary(t =>
                    {
                        var attribute = (UpgradeAttribute)t.GetCustomAttributes(typeof(UpgradeAttribute), false).FirstOrDefault();
                        return attribute.Name + "." + attribute.Version;
                    }
                    , t => t);

            try
            {
                List<IUpgrade> upgrades = new List<IUpgrade>();
                foreach (UpgradeSetting setting in upgradeSettings.Value.Values)
                {
                    var lastVersion = setting.Version;
                    if (setting.Version.CompareTo(setting.LastVersion) < 0)
                    {
                        string s = setting.Name + "." + setting.Version;
                        foreach (var upgrader in upgraders.Keys.Where(p => p.StartsWith(setting.Name) && p.CompareTo(s) > 0))
                        {
                            var t = (IUpgrade)Activator.CreateInstance(upgraders[upgrader], new[] { managerFactory });
                            upgrades.Add(t);
                            t.DoUpgrade(request);
                            if (request.CancelRequest)
                            {
                                break;
                            }
                        }

                        if (request.CancelRequest) { break; }

                    }
                }

                if (!request.CancelRequest)
                {
                    upgrades.ForEach(t =>
                    {
                        if (!request.CancelRequest)
                        {
                            t.PostUpgrade(request);
                            var attr = (UpgradeAttribute)t.GetType().GetCustomAttributes(typeof(UpgradeAttribute), false).FirstOrDefault();
                            var setting = upgradeSettings.Value.Values.FirstOrDefault(p => p.Name == attr.Name);
                            setting.Version = attr.Version;
                        }
                    });
                }

            }

            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                throw;
            }
            finally
            {
                upgradeSettings.Update(opt =>
                {
                    opt.Values = upgradeSettings.Value.Values;
                });
            }
        }
    }
}
