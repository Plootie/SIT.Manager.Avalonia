namespace SIT.Manager.Avalonia.Services
{
    public interface IVersionService
    {
        /// <summary>
        /// Checks and reutrns the installed EFT version
        /// </summary>
        /// <param name="path">The path to check.</param>
        public string GetEFTVersion(string path);
        /// <summary>
        /// Checks and retunrs the installed SIT version
        /// </summary>
        /// <param name="path">The path to check.</param>
        public string GetSITVersion(string path);
    }
}
