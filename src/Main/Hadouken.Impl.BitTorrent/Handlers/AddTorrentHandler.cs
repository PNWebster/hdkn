﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hadouken.Common.BitTorrent;
using Hadouken.Common.Messaging;
using Hadouken.BitTorrent;

namespace Hadouken.Impl.BitTorrent.Handlers
{
    public class AddTorrentHandler : IMessageHandler<AddTorrentMessage>
    {
        private readonly IBitTorrentEngine _torrentEngine;

        public AddTorrentHandler(IBitTorrentEngine torrentEngine)
        {
            _torrentEngine = torrentEngine;
        }

        public void Handle(AddTorrentMessage message)
        {
            var manager = _torrentEngine.AddTorrent(message.Data);

            if (manager != null && !String.IsNullOrEmpty(message.Label))
            {
                manager.Label = message.Label;
            }
        }
    }
}
