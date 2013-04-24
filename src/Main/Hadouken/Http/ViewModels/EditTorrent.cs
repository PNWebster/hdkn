using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.ViewModels
{
    public class EditTorrent
    {
        [JsonProperty("state")]
        public string State { get; set; }
    }
}
