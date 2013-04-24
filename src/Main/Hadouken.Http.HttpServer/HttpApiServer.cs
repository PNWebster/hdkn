using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http.SelfHost;

namespace Hadouken.Http.HttpServer
{
    public class HttpApiServer : IHttpApiServer
    {
        private readonly HttpSelfHostServer _hostServer;

        internal HttpApiServer(HttpSelfHostConfiguration configuration)
        {
            _hostServer = new HttpSelfHostServer(configuration);
        }

        public void Start()
        {
            _hostServer.OpenAsync().Wait();
        }

        public void Stop()
        {
            _hostServer.CloseAsync().Wait();
            _hostServer.Dispose();
        }

        public Uri ListenUri
        {
            get { return null; }
        }
    }
}
