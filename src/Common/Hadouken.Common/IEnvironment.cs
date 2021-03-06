﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;

namespace Hadouken.Common
{
    public interface IEnvironment
    {
        string ConnectionString { get; }
        string HttpBinding { get; }
        NetworkCredential HttpCredentials { get; }
    }
}
