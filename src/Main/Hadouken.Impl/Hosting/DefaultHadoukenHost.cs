﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using Hadouken.Common.Data;
using Hadouken.Common.Http;
using Hadouken.Hosting;

using Hadouken.Data;
using Hadouken.Plugins;
using Hadouken.BitTorrent;
using NLog;
using Hadouken.Common;

namespace Hadouken.Impl.Hosting
{
    [Component]
    public class DefaultHadoukenHost : IHadoukenHost
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IBitTorrentEngine _torrentEngine;
        private readonly IMigrationRunner _migratorRunner;
        private readonly IPluginEngine _pluginEngine;
        private readonly IHttpServer _httpServer;

        public DefaultHadoukenHost(IBitTorrentEngine torrentEngine, IMigrationRunner runner, IPluginEngine pluginEngine, IHttpServerFactory httpServerFactory)
        {
            _torrentEngine = torrentEngine;
            _migratorRunner = runner;
            _pluginEngine = pluginEngine;

            _httpServer = httpServerFactory.Create("http://localhost:8081/", new NetworkCredential("hdkn", "hdkn"));

            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
        }

        void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Logger.FatalException("Unhandled exception.", e.ExceptionObject as Exception);
        }

        public void Load()
        {
            _migratorRunner.Up(this.GetType().Assembly);

            _torrentEngine.Load();
            _pluginEngine.Load();

            _httpServer.Start();
        }

        public void Unload()
        {
            _pluginEngine.UnloadAll();

            _torrentEngine.Unload();

            _httpServer.Stop();
        }
    }
}