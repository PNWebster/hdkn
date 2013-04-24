using System;
using System.Collections.Generic;
using System.IdentityModel.Selectors;
using System.Linq;
using System.Text;

namespace Hadouken.Http.HttpServer
{
    public class IdentityValidator : UserNamePasswordValidator
    {
        public override void Validate(string userName, string password)
        {
            //do nothing
        }
    }
}
