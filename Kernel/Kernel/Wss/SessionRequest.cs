using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;

namespace Kernel.Wss {

    public class SessionRequest : JttpRequest {

        /// <summary>
        /// 获取交互管理器
        /// </summary>
        public dpz.Mvc.Sessions.RedisSessionManager Session { get; private set; }

        /// <summary>
        /// 获取验证状态
        /// </summary>
        public bool Verification { get; private set; }

        public override IRequestResult OnExecuting() {

            this.Session = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false, JRequest.Header.SessionID);
            this.Verification = this.Session.CheckSessionId(JRequest.Header.SessionID, false);
            if (!this.Verification) return Error("交互空闲超时或交互信息无效", Errors.Header_Session_Invalid);

            return base.OnExecuting();
        }

    }
}
