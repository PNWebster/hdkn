﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Hosting
{
    public interface IHadoukenHost
    {
        void Load();
        void Unload();
    }
}
