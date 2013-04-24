using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http.Dependencies;

namespace Hadouken.Http.HttpServer
{
    public class ApiDependencyResolver : IDependencyResolver
    {
        public IDependencyScope BeginScope()
        {
            return this;
        }

        public object GetService(Type serviceType)
        {
            return Kernel.Resolver.Get(serviceType);
        }

        public IEnumerable<object> GetServices(Type serviceType)
        {
            return Kernel.Resolver.GetAll(serviceType);
        }

        public void Dispose()
        {
        }
    }
}
