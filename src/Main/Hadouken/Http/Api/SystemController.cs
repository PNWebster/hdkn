using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.Api
{
    public class SystemController : HttpApiController
    {
        public object Get()
        {
            return new
                {
                    date = DateTime.Now,
                };
        }
    }
}
