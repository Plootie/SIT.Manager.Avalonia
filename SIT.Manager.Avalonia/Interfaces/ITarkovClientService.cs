﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SIT.Manager.Avalonia.Interfaces
{
    public interface ITarkovClientService : IManagedProcess
    {
        void Start(string token, string address);
    }
}
