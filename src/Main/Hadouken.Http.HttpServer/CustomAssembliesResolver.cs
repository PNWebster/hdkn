using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dispatcher;
using System.Reflection;

namespace Hadouken.Http.HttpServer
{
    internal class CustomAssembliesResolver : IAssembliesResolver
    {
        private readonly ICollection<Assembly> _assemblies;

        public CustomAssembliesResolver(Assembly[] assemblies)
        {
            if(assemblies == null)
                throw new ArgumentNullException("assemblies");

            _assemblies = assemblies;
        }

        public ICollection<Assembly> GetAssemblies()
        {
            return _assemblies.ToList();
        }
    }
}
