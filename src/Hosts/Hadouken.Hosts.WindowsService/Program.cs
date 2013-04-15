using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Reflection;
using System.Configuration;
using Hadouken.DI.Ninject;

namespace Hadouken.Hosts.WindowsService
{
    public static class Program
    {
        public static void Main()
        {
            var root = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

            if (String.IsNullOrEmpty(root))
                throw new InvalidDataException("Invalid root path");

            // register base types
            Kernel.SetResolver(new NinjectDependencyResolver());
            Kernel.Register(Directory.GetFiles(root, "*.dll")
                                     .Select(file => AppDomain.CurrentDomain.Load(File.ReadAllBytes(file)))
                                     .ToArray());

            if(Bootstrapper.RunAsConsoleIfRequested<HdknService>())
                return;

            // run the service
            ServiceBase.Run(new ServiceBase[] { new HdknService() });
        }
    }
}
