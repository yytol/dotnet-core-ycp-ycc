using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Wss.Requests {
    public class Desktop : JttpRequest {

        public IRequestResult Get() {
            string host = "" + JReqData.Host;
            //string host = "" + JReqData.Host;
            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                  .Select(p => p.GetIPProperties())
                  .SelectMany(p => p.UnicastAddresses)
                  .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                  .FirstOrDefault()?.Address.ToString();

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosDesktops]&[Host=='{host}'||Host??'{host}|%'||Host??'%|{host}|%'||Host??'%|{host}']}}");
                if (row.IsEmpty) row = dbc.GetGdmlOne($"@{{$[AosDesktops]&[Host=='*']}}");
                if (row.IsEmpty) return Fail("未找到满足条件的云桌面设置");

                row["UrlEntrance"] = row["UrlEntrance"].Replace("${IP}", ip);
                JData.Row = row;
                JData.Host = host;

                //dpz.Jsons.Jttp jttp = new dpz.Jsons.Jttp();
                //jttp.Header.Status = "1";
                //jttp.Header.Type = "Console";
                //string entrance = row["UrlEntrance"] + row["ScriptEntrance"];

                //jttp.Data.Host = host;
                //jttp.Data.Command = $"load-js 'Aos_Desktop_Entrance' '{entrance}'";
                //RenderJttps.Add(jttp);
            }
            return Success();
        }

    }
}
