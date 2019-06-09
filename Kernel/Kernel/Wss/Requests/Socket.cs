using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Kernel.Wss.Requests {
    public class Socket : JttpRequest {

        //获取GUID
        public IRequestResult GetGuid() {
            JData.Guid = this.Socket.Guid;
            return Success();
        }

        //获取GUID
        public IRequestResult GetServerTime() {
            JData.Time = dpz.Time.Now.ToString();
            return Success();
        }

    }
}
