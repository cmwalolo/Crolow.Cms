using System.Collections.Generic;

namespace Crolow.Cms.Server.Core.Models.Configuration
{
    public class DatabaseSettings
    {
        public List<DatabaseSetting> Values { get; set; }
    }

    public class DatabaseSetting
    {
        public DatabaseSetting()
        {

        }
        public string ConnectionString { get; set; }
        public string ClientName { get; set; }
        public string Database { get; set; }
    }
}
