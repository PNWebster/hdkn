using System.Net;
using System.Net.Http;
using Hadouken.BitTorrent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Http;

namespace Hadouken.Http.Api
{
    public class TorrentsController : HttpApiController
    {
        private readonly IBitTorrentEngine _torrentEngine;

        public TorrentsController(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
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
    }
}
