namespace Crolow.Cms.Server.Core.Interfaces.Managers
{
    public interface IModuleProviderManager
    {
        IModuleProvider GetModuleProvider(string moduleName);
    }
}