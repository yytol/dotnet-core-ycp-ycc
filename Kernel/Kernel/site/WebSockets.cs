using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace site {
    public class WebSockets : List<WebSocket> {

        /// <summary>
        /// 添加对象
        /// </summary>
        /// <param name="socket"></param>
        public new void Add(WebSocket socket) {
            bool found = false;
            string guid = "";
            do {
                found = false;
                guid = Guid.NewGuid().ToString();
                for (int i = 0; i < base.Count; i++) {
                    if (base[i].Guid == guid) {
                        found = true;
                        break;
                    }
                }
            } while (found);
            socket.Guid = guid;
            base.Add(socket);
        }

    }
}
