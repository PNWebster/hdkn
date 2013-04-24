using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.HttpServer
{
    internal class FileSystemContent
    {
        public string ContentType { get; set; }
        public byte[] Data { get; set; }
    }
}
