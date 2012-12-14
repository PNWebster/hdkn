﻿using Hadouken.BitTorrent;

namespace Hadouken.Http.Api
{
    [Component]
    [ApiAction("stop")]
    public class StopTorrent : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public StopTorrent(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            string[] hashes = BindModel<string[]>();

            foreach (var hash in hashes)
            {
                if (_torrentEngine.Managers.ContainsKey(hash))
                    _torrentEngine.Managers[hash].Stop();
            }

            return Json(true);
        }
    }
}
