using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.ViewModels
{
    public class PostTorrent
    {
        public string Data { get; set; }

        public string SavePath { get; set; }

        public string Label { get; set; }
    }
}
