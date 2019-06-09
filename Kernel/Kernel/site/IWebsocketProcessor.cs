using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace site {
    public interface IWebsocketProcessor {

        Task Run(WebSocketReceiveResult result, site.WebSocket webSocket, dpz.Jsons.Jttp jttp);

    }
}
