using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hadouken.Reflection;
using System.Net;
using System.IO;
using Hadouken.IO;
using Hadouken.Configuration;
using Ionic.Zip;
using NLog;
using Hadouken.Security;

namespace Hadouken.Http.HttpServer
{
    public class DefaultHttpServer : IHttpServer
    {
        private static readonly int DefaultPort = 8081;
        private static readonly string DefaultBinding = "http://+:{port}/";

        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly IFileSystem _fileSystem;
        private readonly IKeyValueStore _keyValueStore;
        private readonly IRegistryReader _registryReader;

        private HttpListener _listener;
        private string _webUIPath;

        
        public DefaultHttpServer(IKeyValueStore keyValueStore, IRegistryReader registryReader, IFileSystem fileSystem)
        {
            _keyValueStore = keyValueStore;
            _registryReader = registryReader;
            _fileSystem = fileSystem;
        }

        public void Start()
        {
            var binding = GetBinding();

            _listener = new HttpListener();
            _listener.Prefixes.Add(binding);
            _listener.AuthenticationSchemes = AuthenticationSchemes.Basic;

            UnzipWebUI();

            try
            {
                _listener.Start();
            } catch(HttpListenerException e)
            {
                Logger.FatalException("Could not start the HTTP server interface. HTTP server NOT up and running.", e);
                return;
            }

            ReceiveLoop();

            Logger.Info("HTTP server up and running on address " + binding);
        }

        public void Stop()
        {
            if (_listener == null || !_listener.IsListening) return;

            _listener.Stop();
            _listener.Close();
            _listener = null;
        }

        public Uri ListenUri
        {
            get
            {
                if (_listener.IsListening)
                {
                    return new Uri(_listener.Prefixes.First());
                }

                return null;
            }
        }

        private string GetBinding()
        {
            var binding = DefaultBinding;
            var port = _registryReader.ReadInt("webui.port", DefaultPort);

            // Allow overriding from application configuration file.
            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Binding"))
                binding = HdknConfig.ConfigManager["WebUI.Binding"];

            if (HdknConfig.ConfigManager.AllKeys.Contains("WebUI.Port"))
                port = Convert.ToInt32(HdknConfig.ConfigManager["WebUI.Port"]);

            return binding.Replace("{port}", port.ToString());;
        }

        private void ReceiveLoop()
        {
            _listener.BeginGetContext(ar =>
            {
                HttpListenerContext context;

                try
                {
                    context = _listener.EndGetContext(ar);
                }
                catch(Exception)
                {
                    return;
                }

                ReceiveLoop();

                new Task(() => ProcessRequest(context)).Start();

            }, null);
        }

        private void ProcessRequest(HttpListenerContext context)
        {
            try
            {
                Logger.Trace("Incoming request to {0}", context.Request.Url);

                if(IsAuthenticatedUser(context))
                {
                    var result = CheckFileSystem(context);

                    if (result != null)
                    {
                        context.Response.ContentType = result.ContentType;
                        context.Response.OutputStream.Write(result.Data, 0, result.Data.Length);
                    }
                    else
                    {
                        context.Response.StatusCode = 404;
                        context.Response.StatusDescription = "404 - File not found";
                    }
                }
                else
                {
                    Logger.Info("Unauthorized user request.");

                    OnUnauthorized(context);
                }

                context.Response.OutputStream.Close();
                context.Response.Close();
            }
            catch(Exception e)
            {
                OnError(context, e);
            }
        }

        private void OnUnauthorized(HttpListenerContext context)
        {
            context.Response.StatusCode = 401;
        }

        private void OnError(HttpListenerContext context, Exception e)
        {
            var page = "<h1>Internal Server Error</h1>";
            page = page + "<h2><pre>" + context.Request.Url + "</pre></h2>";
            page = page + "<div><pre>" + e.StackTrace + "</pre></div>";

            var data = Encoding.UTF8.GetBytes(page);

            context.Response.ContentType = "text/html";
            context.Response.OutputStream.Write(data, 0, data.Length);

            context.Response.OutputStream.Close();
            context.Response.Close();
        }

        private void UnzipWebUI()
        {
            _webUIPath = HdknConfig.GetPath("Paths.WebUI");

            string uiZip = Path.Combine(_webUIPath, "webui.zip");

            Logger.Debug("Checking if webui.zip exists at {0}", uiZip);

            if (_fileSystem.FileExists(uiZip))
            {
                string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                _fileSystem.CreateDirectory(path);

                Logger.Info("Extracting webui.zip to {0}", path);

                using (var zip = ZipFile.Read(uiZip))
                {
                    zip.ExtractAll(path);
                }

                _webUIPath = path;
            }
            else
            {
                _webUIPath = Path.Combine(_webUIPath, "WebUI");
            }
        }

        private FileSystemContent CheckFileSystem(HttpListenerContext context)
        {
            string path = _webUIPath + (context.Request.Url.AbsolutePath == "/" ? "/index.html" : context.Request.Url.AbsolutePath);

            if (_fileSystem.FileExists(path))
            {
                string contentType = "text/html";

                switch (Path.GetExtension(path))
                {
                    case ".css":
                        contentType = "text/css";
                        break;

                    case ".js":
                        contentType = "text/javascript";
                        break;

                    case ".png":
                        contentType = "image/png";
                        break;

                    case ".gif":
                        contentType = "image/gif";
                        break;
                }

                return new FileSystemContent { Data = _fileSystem.ReadAllBytes(path), ContentType = contentType };
            }

            return null;
        }

        private bool IsAuthenticatedUser(HttpListenerContext context)
        {
            if (context.User.Identity.IsAuthenticated)
            {
                var id = (HttpListenerBasicIdentity)context.User.Identity;

                var usr = _keyValueStore.Get<string>("auth.username");
                var pwd = _keyValueStore.Get<string>("auth.password");

                return (id.Name == usr && Hash.Generate(id.Password) == pwd);
            }

            return false;
        }
    }
}
