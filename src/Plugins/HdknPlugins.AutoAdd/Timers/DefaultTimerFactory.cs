﻿using Hadouken.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HdknPlugins.AutoAdd.Timers
{
    [Component]
    public class DefaultTimerFactory : ITimerFactory
    {
        public ITimer CreateTimer()
        {
            return new ThreadedTimer();
        }
    }
}
