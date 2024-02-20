using SIT.Manager.Avalonia.Interfaces;
using SIT.Manager.Avalonia.ManagedProcess;
using SIT.Manager.Avalonia.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Classes
{
    public class ZlibCompressionService : IZlibCompressionService
    {
        private Assembly? _zlibAssembly;
        private MethodInfo? compressToBytesMethodInfo;
        public ZlibCompressionService(IManagerConfigService configService)
        {
            if (string.IsNullOrEmpty(configService.Config.InstallPath))
                configService.ConfigChanged += LoadZlibFromConfig;
            else
                LoadZlibFromConfig(null, configService.Config);

        }

        public byte[] CompressToBytes(string data, ZlibCompression compressionProfile, Encoding? encoding = null)
        {
            return (byte[]?)compressToBytesMethodInfo?.Invoke(null, new object[] { data, compressionProfile, encoding ?? Encoding.UTF8 }) ?? [];
        }

        private void LoadZlibFromConfig(object? sender, ManagerConfig e)
        {
            if (!string.IsNullOrEmpty(e.InstallPath))
            {
                string assemblyPath = Path.Combine(e.InstallPath, "EscapeFromTarkov_Data", "Managed", "bsg.componentace.compression.libs.zlib.dll");
                _zlibAssembly = Assembly.LoadFrom(assemblyPath);
                Type? SimpleZlib = _zlibAssembly?.GetType("ComponentAce.Compression.Libs.zlib.SimpleZlib");
                compressToBytesMethodInfo = SimpleZlib?.GetMethod("CompressToBytes", [typeof(string), typeof(int), typeof(Encoding)]);

                if (sender is IManagerConfigService configService && compressToBytesMethodInfo != null)
                    configService.ConfigChanged -= LoadZlibFromConfig;
            }
        }
    }
}
