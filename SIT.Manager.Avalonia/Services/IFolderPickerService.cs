using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Services
{
    public interface IFolderPickerService
    {
        Task<IStorageFolder?> OpenFolderAsync();
    }
}
