using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Hadouken.Http.ViewModels
{
    public class TorrentProperties
    {
        [JsonProperty("trackers")]
        public string[][] Trackers { get; set; }

        [JsonProperty("ulrate")]
        public int? MaxUploadSpeed { get; set; }

        [JsonProperty("dlrate")]
        public int? MaxDownloadSpeed { get; set; }

        [JsonProperty("superseed")]
        public bool? InitialSeedingEnabled { get; set; }

        [JsonProperty("dht")]
        public bool? UseDht { get; set; }

        [JsonProperty("pex")]
        public bool? EnablePeerExchange { get; set; }

        [JsonProperty("ulslots")]
        public int? UploadSlots { get; set; }
    }
}
