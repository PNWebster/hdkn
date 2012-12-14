﻿using Hadouken.BitTorrent;

namespace Hadouken.Http.Api
{
    [Component]
    [ApiAction("start")]
    public class StartTorrent : ApiAction
    {
        private IBitTorrentEngine _torrentEngine;

        public StartTorrent(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public override ActionResult Execute()
        {
            var hashes = BindModel<string[]>();

            foreach (var hash in hashes)
            {
                if (_torrentEngine.Managers.ContainsKey(hash))
                    _torrentEngine.Managers[hash].Start();
            }

            return Json(true);
        }
    }
}
