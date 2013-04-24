using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http
{
    public interface IHttpApiServer
    {
        void Start();
        void Stop();
    }
}
