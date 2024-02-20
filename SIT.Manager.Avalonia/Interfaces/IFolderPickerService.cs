using Avalonia.Platform.Storage;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface IFolderPickerService
    {
        Task<IStorageFolder?> OpenFolderAsync();
    }
}
