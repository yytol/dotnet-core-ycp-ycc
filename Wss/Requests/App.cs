using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dpz;

namespace Kernel.Wss.Requests {
    public class App : SessionRequest {

        //获取用户授权列表
        public IRequestResult GetList() {

            if (!this.Verification) return Error("空闲超时或交互标识无效", 0);

            long aid = ((string)JRequest.Data["AuthID"]).ToLong();

            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                  .Select(p => p.GetIPProperties())
                  .SelectMany(p => p.UnicastAddresses)
                  .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                  .FirstOrDefault()?.Address.ToString();

            long uid = Session["User_ID"].ToLong();
            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var list = dbc.GetGdmlList($"@{{$[AosUserApps]$[AosApps].[*]&[AosUserApps.UserID=='{uid}'&&AosUserApps.AuthID=='{aid}'&&AosUserApps.AppID==AosApps.ID]+[AosApps.Index]}}");
                JData.List = new List<dpz.Dynamic>();
                List<dpz.Dynamic> jList = JData.List;
                foreach (var row in list) {

                    //进行数据过滤
                    row["Path"] = row["Path"].Replace("${IP}", ip);

                    var jRow = new dpz.Dynamic();
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
                                jRow[item.Key] = item.Value;
                                break;
                        }
                    }
                    jList.Add(jRow);
                }
            }

            return Success();
        }

    }
}
