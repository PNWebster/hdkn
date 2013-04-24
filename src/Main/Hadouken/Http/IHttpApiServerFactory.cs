﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Hadouken.Http
{
    public interface IHttpApiServerFactory : IComponent
    {
        IHttpApiServer CreateHttpApiServer(Uri baseAddress, params Assembly[] assemblies);
    }
}
