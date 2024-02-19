namespace SIT.Manager.Avalonia.Services
{
    public interface ITarkovClientService
    {
        /// <summary>
        /// Clear the EFT local cache and the mods cache.
        /// </summary>
        void ClearCache();
        /// <summary>
        /// Clear EFT local cache.
        /// </summary>
        void ClearLocalCache();
    }
}
