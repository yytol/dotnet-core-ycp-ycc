using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dpz;

namespace Kernel.Api {
    [Route("Api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase {

        private string GetInfoBySid(string sid, string randString, string md5) {

            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();

            if (sid.IsNone()) {
                res.Header.Status = "0";
                res.Message = "交互标识无效";
                return res.ToJson();
            }

            if (randString.IsNone()) {
                res.Header.Status = "0";
                res.Message = "缺少身份授权所需的随机字符串";
                return res.ToJson();
            }

            if (randString.Length < 32) {
                res.Header.Status = "0";
                res.Message = "为保证通讯安全,身份授权所需的随机字符串最少长度为32位";
                return res.ToJson();
            }

            if (md5.IsNone()) {
                res.Header.Status = "0";
                res.Message = "缺少身份授权所需的验证码";
                return res.ToJson();
            }

            using (dpz.Mvc.Sessions.RedisSessionManager redis = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false, sid)) {
                //redis.CreateSessionId();
                if (!redis.CheckSessionId(sid, false)) {
                    res.Header.Status = "0";
                    res.Message = "交互标识无效";
                    return res.ToJson();
                }

                long uid = redis["User_ID"].ToLong();
                string sessionKey = redis["Session_Key"];

                if (uid <= 0) {
                    res.Header.Status = "0";
                    res.Message = "用户尚未登录或登陆状态失效";
                    return res.ToJson();
                }

                using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                    var row = dbc.GetGdmlOne($"@{{$[AosUsers]&[ID=='{uid}']}}");
                    if (row.IsEmpty) {
                        res.Header.Status = "0";
                        res.Message = "未找到用户信息";
                        return res.ToJson();
                    }

                    string name = row["Name"];
                    string userMD5 = ("name=" + name + "&str=" + randString + "&key=" + sessionKey).GetMD5();
                    if (md5 != userMD5) {
                        res.Header.Status = "0";
                        res.Message = "MD5验证失败";
                        return res.ToJson();
                    }

                    foreach (var item in row) {
                        if (item.Key != "Pwd")
                            res.Data[item.Key] = item.Value;
                    }
                }

                res.Header.Status = "1";
                return res.ToJson();
            }

        }

        [HttpGet("GetInfo")]
        [Yapi("以Get方式获取登陆用户信息")]
        public string GetInfo() {
            string sid = Request.Query["sid"];
            string str = Request.Query["str"];
            string md5 = Request.Query["md5"];
            return GetInfoBySid(sid, str, md5);
        }

        [HttpPost("GetInfo")]
        [Yapi("以Post方式获取登陆用户信息")]
        public string PostInfo() {
            string sid = Request.Form["sid"];
            string str = Request.Form["str"];
            string md5 = Request.Form["md5"];
            return GetInfoBySid(sid, str, md5);
        }

        [HttpGet]
        [Yapi("默认交互，返回空内容")]
        public string Get() {
            return "{}";
            //return obj.ToJson();
        }


    }
}