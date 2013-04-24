using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public interface IUriBuilder : IComponent
    {
        Uri Build(params string[] pathTree);
    }
}
