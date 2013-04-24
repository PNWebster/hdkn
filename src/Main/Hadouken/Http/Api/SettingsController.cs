using System.Net;
using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Web.Http;

namespace Hadouken.Http.Api
{
    public class SettingsController : HttpApiController
    {
        private readonly IDataRepository _repository;
        private readonly IKeyValueStore _keyValueStore;

        public SettingsController(IDataRepository repository, IKeyValueStore keyValueStore)
        {
            _repository = repository;
            _keyValueStore = keyValueStore;
        }

        public object Get()
        {
            return new
                {
                    settings = (from setting in _repository.List<Setting>()
                                where setting.Key != "auth.password"
                                select new []
                                    {
                                        setting.Key.ToLowerInvariant(),
                                        GetSettingType(setting.Type),
                                        Newtonsoft.Json.JsonConvert.DeserializeObject(setting.Value),
                                        setting.Permissions,
                                        setting.Options
                                    })
                };
        }

        public HttpResponseMessage Post([FromBody] Dictionary<string, object> data)
        {
            foreach (var key in data.Keys)
            {
                _keyValueStore.Set(key, data[key]);
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        private int GetSettingType(string type)
        {
            switch (type)
            {
                case "System.Int32":
                    return 0;

                case "System.Boolean":
                    return 1;

                case "System.String":
                    return 2;

                default:
                    return -1;
            }
        }
    }
}
