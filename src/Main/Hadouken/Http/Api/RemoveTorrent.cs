﻿using Hadouken.BitTorrent;

namespace Hadouken.Http.Api
{
    [Component]
    [ApiAction("remove")]
    public class RemoveTorrent : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public RemoveTorrent(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string[] hashes = BindModel<string[]>();

            foreach (var hash in hashes)
            {
                if (_torrentEngine.Managers.ContainsKey(hash))
                {
                    var torrent = _torrentEngine.Managers[hash];

                    _torrentEngine.RemoveTorrent(torrent);
                }
            }

            return Json(true);
        }
    }
}
