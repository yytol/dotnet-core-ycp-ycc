using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kernel.Api {

    [Route("Api/[controller]")]
    [ApiController]
    public class YccController : ControllerBase {

        [HttpGet("GetConfig")]
        [Yapi("获取配置信息")]
        public string GetConfig() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();

            res.Header.Time = "" + dpz.Time.Now.ToTimeStamp();

            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();
            //res.Data.Sid = redis.SessionID;
            //res.Data.Time = time;
            var websocket = new dpz.Dynamic();
            res.Data.websocket = websocket;
            websocket["url"] = site.Config.Url.Websocket.Replace("${IP}", ip);

            res.Header.Status = "1";
            return res.ToJson();

            //return obj.ToJson();
        }

        [HttpGet("CreateNew")]
        [Yapi("创建一个交互标识")]
        public string CreateNew() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();

            res.Header.Time = "" + dpz.Time.Now.ToTimeStamp();

            using (dpz.Mvc.Sessions.RedisSessionManager redis = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false)) {
                redis.CreateSessionId();

                string time = "" + dpz.Time.Now.ToTimeStamp();

                redis["Session_Time"] = time;

                res.Data.Sid = redis.SessionID;
                res.Data.Time = time;
            }

            res.Header.Status = "1";
            return res.ToJson();

            //return obj.ToJson();
        }

        [HttpGet]
        [Yapi("默认交互，返回空内容")]
        public string Get() {
            return "{}";
            //return obj.ToJson();
        }

    }
}