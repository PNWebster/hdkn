﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Common.Http.Mvc
{
    public class HttpDeleteAttribute : HttpMethodAttribute
    {
        public HttpDeleteAttribute(string route) : base(route, "DELETE")
        {
        }
    }
}