using System;
using System.Collections.Generic;
using System.Text;

namespace Crolow.Cms.Server.Core.Models.Configuration
{
    public class UpgradeSettings
    {
        public List<UpgradeSetting> Values { get; set; }
    }

    public class UpgradeSetting
    {
        public UpgradeSetting()
        {

        }

        public string Name { get; set; }
        public string Version { get; set; }
        public string LastVersion { get; set; } 
    }
}
