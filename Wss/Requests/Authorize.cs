using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dpz;

namespace Kernel.Wss.Requests {
    public class Authorize : SessionRequest {

        //获取GUID
        public IRequestResult Create() {
            if (!this.Verification) return Error("空闲超时或交互标识无效", 0);

            long aid = (JReqData.AuthID as string).ToLong();
            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                string code = "";
                do {
                    code = Guid.NewGuid().ToString().Replace("-", "");
                } while (!dbc.GetOne("@{$[AosAuthorizeCode].[*]&[Code=='" + code + "']}").IsEmpty);
                dbc.Exec("+{$[AosAuthorizeCode].[AuthID='" + aid + "'].[SessionID='" + this.Session.SessionID + "'].[Code='" + code + "'].[Token=''].[EffectiveTime='" + dpz.Time.New(DateTime.Now.AddMinutes(30)).ToTimeStamp() + "'].[Status='0']}");
                //res = redirect + "?code=" + code;
                JData.Code = code;
            }

            return Success();
        }

        //更新交互
        public IRequestResult Keep() {
            using (dpz.Mvc.Sessions.RedisSessionManager redis = new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString, false)) {
                redis.CreateSessionId();

                if (!redis.CheckSessionId(JRequest.Header.SessionID)) {
                    //await Send(null, 0, $"交互标识不存在或已过期");
                    return Fail("交互标识不存在或已过期");
                }
                //await Send(null, 1, "", 0, "", redis.SessionID);
            }
            return Success();
        }

        //获取用户授权列表
        public IRequestResult GetList() {
            if (!this.Verification) return Error("空闲超时或交互标识无效", 0);

            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                  .Select(p => p.GetIPProperties())
                  .SelectMany(p => p.UnicastAddresses)
                  .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                  .FirstOrDefault()?.Address.ToString();

            long uid = Session["User_ID"].ToLong();
            long desktopId = ((string)JReqData.DesktopID).ToLong();

            if (desktopId <= 0) return Fail("未指定一个有效的云桌面信息");

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var list = dbc.GetGdmlList($"@{{$[AosUserAuthorize]$[AosDesktopAuthorizes]$[AosAuthorize].[*]&[AosUserAuthorize.UserID=='{uid}'&&AosUserAuthorize.AuthID==AosAuthorize.ID&&AosDesktopAuthorizes.AuthID==AosAuthorize.ID&&AosDesktopAuthorizes.DesktopID=='{desktopId}'&&AosDesktopAuthorizes.Compatibility=='1']}}");
                JData.List = new List<dpz.Dynamic>();
                List<dpz.Dynamic> jList = JData.List;
                foreach (var row in list) {

                    //进行数据过滤
                    row["UrlEntrance"] = row["UrlEntrance"].Replace("${IP}", ip);
                    long userCreate = row["CreateUserID"].ToLong();
                    //int active = row["Active"].ToInteger();
                    //row["InfoStatus"] = active > 0 ? "√" : "";

                    var jRow = new dpz.Dynamic();
                    foreach (var item in row) {
                        switch (item.Key) {
                            //case "DBType":
                            case "DBIP":
                            case "DBPort":
                            case "DBUser":
                            case "DBPwd":
                            case "DBPath":
                            //case "Code":
                            case "SecurityKey":
                                break;
                            default:
                                jRow[item.Key] = item.Value;
                                break;
                        }
                    }
                    jList.Add(jRow);
                }
            }

            return Success();
        }

        //激活授权
        public IRequestResult Active() {
            if (!this.Verification) return Error("空闲超时或交互标识无效", 0);

            long uid = Session["User_ID"].ToLong();
            string aid = JRequest.Data["AuthID"];
            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosUserAuthorize]$[AosAuthorize].[*]&[UserID=='{uid}'&&AuthID=='{aid}']}}");
                if (row.IsEmpty) {
                    return Fail($"无操作权限");
                }
                dbc.ExecGdml($"!{{$[AosUserAuthorize].[Active='0']&[UserID=='{uid}']}}");
                dbc.ExecGdml($"!{{$[AosUserAuthorize].[Active='1']&[ID=='{row["ID"]}']}}");
            }

            return Success();
        }

        //激活授权
        public IRequestResult GetActive() {
            if (!this.Verification) return Error("空闲超时或交互标识无效", 0);

            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                  .Select(p => p.GetIPProperties())
                  .SelectMany(p => p.UnicastAddresses)
                  .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                  .FirstOrDefault()?.Address.ToString();

            long uid = Session["User_ID"].ToLong();
            //string aid = JRequest.Data["AuthID"];
            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosUserAuthorize]$[AosAuthorize].[*]&[AosUserAuthorize.UserID=='{uid}'&&AosUserAuthorize.Active=='1'&&AosUserAuthorize.AuthID==AosAuthorize.ID]}}");
                if (row.IsEmpty) {
                    return Fail($"无可操作对象,请先激活一个可操作的对象");
                }

                //进行数据过滤
                row["UrlEntrance"] = row["UrlEntrance"].Replace("${IP}", ip);
                long userCreate = row["CreateUserID"].ToLong();
                if (userCreate != uid) {
                    row["Code"] = "";
                    row["SecurityKey"] = "";
                }

                foreach (var item in row) {
                    switch (item.Key) {
                        case "DBType":
                        case "DBIP":
                        case "DBPort":
                        case "DBUser":
                        case "DBPwd":
                        case "DBPath":
                            break;
                        default:
                            JData[item.Key] = item.Value;
                            break;
                    }
                }
            }

            return Success();
        }

    }
}
