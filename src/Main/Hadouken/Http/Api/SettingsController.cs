using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.Api
{
    public class SettingsController : HttpApiController
    {
        private readonly IDataRepository _repository;

        public SettingsController(IDataRepository repository)
        {
            _repository = repository;
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
