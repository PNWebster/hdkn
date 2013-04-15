﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Configuration;

namespace Hadouken.UnitTests
{
    public class MemoryConfigManager : IConfigManager
    {
        private static readonly string c = "Data Source=test.db;Version=3;New=True";
        private IDictionary<string, string> config = new Dictionary<string, string>();

        public MemoryConfigManager()
        {
            config.Add("Paths.Data", "Data");
            config.Add("Paths.Logs", "Logs");
            config.Add("Paths.Plugins", "Plugins");
            config.Add("Paths.WebUI", "../../../../WebUI");

            config.Add("WebUI.Binding", "http://localhost:{port}/");
            config.Add("WebUI.Port", "8081");
        }

        public string ConnectionString
        {
            get { return c; }
        }

        public string[] AllKeys
        {
            get { return config.Keys.ToArray(); }
        }

        public string this[string key]
        {
            get { return config[key]; }
        }
    }
}
