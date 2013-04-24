using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Plugins;

namespace Hadouken.Http.Api
{
    public class PluginsController : HttpApiController
    {
        private readonly IPluginEngine _pluginEngine;

        public PluginsController(IPluginEngine pluginEngine)
        {
            _pluginEngine = pluginEngine;
        }

        public object Get()
        {
            return new
                {
                    plugins = (from plugin in _pluginEngine.Managers
                               select new
                                   {
                                       plugin.Value.Name,
                                       plugin.Value.Version
                                   }).ToArray()
                };
        }
    }
}
