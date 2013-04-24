using System.IO;
using System.Net;
using System.Net.Http;
using Hadouken.BitTorrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;
using Hadouken.Http.ViewModels;

namespace Hadouken.Http.Api
{
    public class TorrentsController : HttpApiController
    {
        private readonly IBitTorrentEngine _torrentEngine;

        public TorrentsController(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public object Get()
        {
            return new
                {
                    label = (from l in _torrentEngine.Managers.Values.GroupBy(m => m.Label)
                             where !String.IsNullOrEmpty(l.Key)
                             select new object[]
                                 {
                                     l.Key,
                                     l.Count()
                                 }),
                    torrents = (from t in _torrentEngine.Managers.Values
                                select new object[]
                                    {
                                        t.InfoHash,
                                        t.State,
                                        t.Torrent.Name,
                                        t.Torrent.Size,
                                        (int) t.Progress*10,
                                        t.DownloadedBytes,
                                        t.UploadedBytes,
                                        (t.DownloadedBytes == 0 ? 0 : (int) ((t.UploadedBytes/t.DownloadedBytes)*10)),
                                        t.UploadSpeed,
                                        t.DownloadSpeed,
                                        t.ETA.TotalSeconds,
                                        t.Label,
                                        t.Peers.Count(p => !p.IsSeeder),
                                        t.Trackers.Sum(tr => tr.Incomplete),
                                        t.Peers.Count(p => p.IsSeeder),
                                        t.Trackers.Sum(tr => tr.Complete),
                                        -1, // availability
                                        -1, // queue position
                                        t.RemainingBytes,
                                        "", // download url
                                        "", // rss feed url
                                        (t.State == TorrentState.Error ? "Error: --" : ""),
                                        -1, // stream id
                                        t.StartTime.ToUnixTime(),
                                        (t.CompletedTime.HasValue ? t.CompletedTime.Value.ToUnixTime() : -1),
                                        "", // app update url
                                        (t.Torrent.Files.Length > 1
                                             ? t.SavePath
                                             : Path.Combine(t.SavePath, t.Torrent.Name)),
                                        t.Complete
                                    })
                };
        }

        public HttpResponseMessage Put(string id, [FromBody] EditTorrent data)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var m = _torrentEngine.Managers[id];

            switch ((data.State ?? "").ToLowerInvariant())
            {
                case "start":
                    m.Start();
                    break;

                case "stop":
                    m.Stop();
                    break;

                case "pause":
                    m.Pause();
                    break;
            }

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        public HttpResponseMessage Delete(string id)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var m = _torrentEngine.Managers[id];
            _torrentEngine.RemoveTorrent(m);

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [ActionName("properties")]
        public object GetProperties(string id)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                    {
                        ReasonPhrase = "No torrent found"
                    });

            var torrent = _torrentEngine.Managers[id];

            return new
                {
                    timestamp = DateTime.Now.ToUnixTime(),
                    infoHash = id,
                    props = new
                        {
                            hash = torrent.InfoHash,
                            trackers = "",
                            ulrate = torrent.Settings.MaxUploadSpeed,
                            dlrate = torrent.Settings.MaxDownloadSpeed,
                            superseed = torrent.Settings.InitialSeedingEnabled,
                            dht = torrent.Settings.UseDht,
                            pex = torrent.Settings.EnablePeerExchange,
                            seed_override = false,
                            seed_ratio = 0,
                            seed_time = 0,
                            ulslots = torrent.Settings.UploadSlots,
                            seed_num = 0
                        }
                };
        }

        [HttpPut]
        [ActionName("properties")]
        public HttpResponseMessage PutProperties(string id, [FromBody]TorrentProperties props)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                return new HttpResponseMessage(HttpStatusCode.NotFound);

            var m = _torrentEngine.Managers[id];

            if (props.EnablePeerExchange != null)    m.Settings.EnablePeerExchange = props.EnablePeerExchange.Value;
            if (props.InitialSeedingEnabled != null) m.Settings.InitialSeedingEnabled = props.InitialSeedingEnabled.Value;
            if (props.MaxDownloadSpeed != null)      m.Settings.MaxDownloadSpeed = props.MaxDownloadSpeed.Value;
            if (props.MaxUploadSpeed != null)        m.Settings.MaxUploadSpeed = props.MaxUploadSpeed.Value;
            //if (props.Trackers != null)            parse trackers
            if (props.UploadSlots != null)           m.Settings.UploadSlots = props.UploadSlots.Value;
            if (props.UseDht != null)                m.Settings.UseDht = props.UseDht.Value;

            return new HttpResponseMessage(HttpStatusCode.NoContent);
        }

        [HttpGet]
        [ActionName("peers")]
        public object GetPeers(string id)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "No torrent found"
                });

            return new
                {
                    timestamp = DateTime.Now.ToUnixTime(),
                    infoHash = id,
                    peers = (from peer in _torrentEngine.Managers[id].Peers
                             select new object[]
                                 {
                                     "00", // country
                                     peer.EndPoint.Address.ToString(), // ip
                                     peer.ReverseDns, // reverse dns
                                     0, // utp
                                     peer.EndPoint.Port, // port
                                     peer.ClientSoftware, // client software
                                     "", // flags
                                     peer.Progress, // progress
                                     peer.DownloadSpeed, // download speed
                                     peer.UploadSpeed, // upload speed
                                     -1, // requests in
                                     -1, // requests out
                                     -1, // waited
                                     peer.UploadedBytes, // uploaded
                                     peer.DownloadedBytes, // downloaded
                                     peer.HashFails, // hash errors
                                     -1, // peer dl
                                     -1, // max up
                                     -1, // max down
                                     -1, // queued
                                     -1, // inactive
                                     -1, // relevance
                                 })
                };
        }

        [HttpGet]
        [ActionName("files")]
        public object GetFiles(string id)
        {
            if (!_torrentEngine.Managers.ContainsKey(id))
                throw new HttpResponseException(new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "No torrent found"
                });

            return new
                {
                    files = new object[]
                        {
                            id,
                            (from file in _torrentEngine.Managers[id].Torrent.Files
                             select new object[]
                                 {
                                     file.Path,
                                     file.Length,
                                     file.BytesDownloaded,
                                     file.Priority
                                 })
                        }
                };
        }
    }
}
