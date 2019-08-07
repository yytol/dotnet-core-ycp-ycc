using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dpz;

namespace Kernel.Wss.Requests {
    public class Session : JttpRequest {

        //获取GUID
        public IRequestResult Create() {
            using (dpz.Mvc.Sessions.RedisSessionManager redis = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false)) {
                redis.CreateSessionId();

                string time = "" + dpz.Time.Now.ToTimeStamp();
                string key = Guid.NewGuid().ToString().Replace("-", "");

                redis["Session_Time"] = time;
                redis["Session_Key"] = key;

                JData.Sid = redis.SessionID;
                JData.Key = key;
                JData.Time = time;
            }
            return Success();
        }

        //更新交互
        public IRequestResult Keep() {
            using (dpz.Mvc.Sessions.RedisSessionManager redis = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false)) {
                redis.CreateSessionId();

                if (!redis.CheckSessionId(JRequest.Header.SessionID)) {
                    //await Send(null, 0, $"交互标识不存在或已过期");
                    return Fail();
                }
                //await Send(null, 1, "", 0, "", redis.SessionID);
            }
            return Success();
        }

    }
}
