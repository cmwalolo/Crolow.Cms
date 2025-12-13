using Crolow.Cms.Server.Core.Models.Databases;

namespace Crolow.Cms.Server.Core.Interfaces.Managers;

public interface IDatabaseContextManager
{
    void Dispose();
    IDatabaseContext GetContext(DataStore store);
}