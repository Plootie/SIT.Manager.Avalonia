using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Classes
{
    public struct TarkovEdition(string edition, string? description = null)
    {
        string Edition { get; } = edition;
        string Description = description ?? string.Empty;
    }
}
