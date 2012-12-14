﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Configuration;
using Hadouken.Data;
using Hadouken.Data.Models;
using System.Web.Script.Serialization;
using System.Linq.Expressions;
using Hadouken.Messaging;
using Hadouken.Messages;

namespace Hadouken.Impl.Config
{
    [Component]
    public class DefaultKeyValueStore : IKeyValueStore
    {
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private readonly IDataRepository _data;
        private readonly IMessageBus _bus;

        public DefaultKeyValueStore(IDataRepository data, IMessageBus bus)
        {
            _data = data;
            _bus = bus;
        }

        public object Get(string key)
        {
            return Get(key, null);
        }

        public object Get(string key, object defaultValue)
        {
            var setting = _data.Single<Setting>(s => s.Key == key);

            if (setting == null)
            {
                Set(key, defaultValue);
                return Get(key, defaultValue);
            }

            return _serializer.Deserialize(key, Type.GetType(setting.Type));
        }

        public IDictionary<string, object> Get(Func<string, bool> filter)
        {
            var result = new Dictionary<string, object>();

            var settings = _data.List<Setting>().Where(s => filter.Invoke(s.Key));

            foreach (var setting in settings)
            {
                result.Add(setting.Key, _serializer.Deserialize(setting.Value, Type.GetType(setting.Type)));
            }

            return result;
        }

        public T Get<T>(string key)
        {
            return Get<T>(key, default(T));
        }

        public T Get<T>(string key, T defaultValue)
        {
            var setting = _data.Single<Setting>(s => s.Key == key);

            if (setting == null)
            {
                Set(key, defaultValue);
                return Get<T>(key, defaultValue);
            }

            return _serializer.Deserialize<T>(setting.Value);
        }

        public void Set(string key, object value)
        {
            if(value == null)
                throw new ArgumentNullException("value");

            var setting = _data.Single<Setting>(s => s.Key == key);
            
            if (setting == null)
            {
                setting = new Setting { Key = key, Type = value.GetType().FullName };
            }

            setting.Value = _serializer.Serialize(value);

            _data.SaveOrUpdate(setting);

            // Send ISettingChanged message
            _bus.Send<ISettingChanged>(msg =>
                                           {
                                               msg.Key = setting.Key;
                                               msg.NewValue = value;
                                           });
        }
    }
}
