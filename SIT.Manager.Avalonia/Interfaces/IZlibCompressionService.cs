﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface IZlibCompressionService
    {
        public byte[] CompressToBytes(string data, ZlibCompression compressionProfile, Encoding? encoding = null);
    }

    public enum ZlibCompression
    {
        BestSpeed = 1,
        BestCompression = 9
    }
}
