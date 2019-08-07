using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using dpz.Jsons;
using site;

namespace Kernel.Wss {
    public class WebsocketProcessor : site.IWebsocketProcessor {

        //获取GUID
        private async Task SendError(WebSocketReceiveResult result, site.WebSocket socket, string msg, int code = 0, string tp = "") {
            using (dpz.Jsons.Jttp jttp = new dpz.Jsons.Jttp()) {
                jttp.Header.Ver = "1.0";
                jttp.Header.Type = tp;
                jttp.Header.Time = "" + dpz.Time.Now.ToTimeStamp();
                jttp.Header.Status = "-1";
                jttp.Header.Error = "" + code;
                jttp.Message = msg;

                byte[] bs = System.Text.Encoding.UTF8.GetBytes(jttp.ToJson());
                await socket.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
        }

        public async Task Run(WebSocketReceiveResult result, site.WebSocket webSocket, Jttp jttp) {
            //throw new NotImplementedException();
            var ass = Assembly.GetExecutingAssembly();

            //处理特殊交互
            //if (jttp.Header.Type == "Help") {
            //    var reqs = ass.GetTypes().Where(req => req.IsAssignableFrom(typeof(JttpRequest)));
            //    using (dpz.Jsons.Jttp res = new dpz.Jsons.Jttp()) {
            //        res.Header.Ver = "1.0";
            //        res.Header.Type = jttp.Header.Type;
            //        res.Header.Time = "" + dpz.Time.Now.ToTimeStamp();
            //        res.Header.Status = "1";

            //        byte[] bs = System.Text.Encoding.UTF8.GetBytes(jttp.ToJson());
            //        await webSocket.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);

            //        var objs = new List<dpz.Dynamic>();
            //        res.Data.Requests = objs;
            //        foreach (var req in reqs) {
            //            dynamic obj = new dpz.Dynamic();
            //            obj.Name = req.Name;
            //            objs.Add(obj);
            //        }
            //    }
            //    return;
            //}

            string[] tps = jttp.Header.Type.Split('.');
            JttpRequest request = null;

            if (tps.Length <= 1) {
                await SendError(result, webSocket, $"不支持的交互类型'{jttp.Header.Type}'"); return;
            }

            //switch (tps[0]) {
            //    case "Socket": request = new Requests.Socket(); break;
            //    case "Session": request = new Requests.Session(); break;
            //    default: await SendError(result, webSocket, $"不支持的交互类型'{jttp.Header.Type}'"); return;
            //}
            var fullName = "Kernel.Wss.Requests." + tps[0];
            var requests = ass.GetTypes();

            for (int i = 0; i < requests.Length; i++) {
                var req = requests[i];
                if (req.FullName == fullName) {

                    //新建反射对象
                    request = ass.CreateInstance(fullName) as JttpRequest;
                    request.Init(result, webSocket, jttp);

                    var methods = req.GetMethods();
                    for (int j = 0; j < methods.Length; j++) {
                        var method = methods[j];
                        if (method.Name == tps[1]) {

                            //处理前置事件
                            IRequestResult resExecuting = request.OnExecuting();
                            if (resExecuting != null) {
                                resExecuting.SetResult(request.JResponse);
                            } else {
                                IRequestResult res = method.Invoke(request, null) as IRequestResult;
                                res.SetResult(request.JResponse);

                                //处理后置事件
                                IRequestResult resExecuted = request.OnExecuted();
                                if (resExecuted != null) {
                                    resExecuted.SetResult(request.JResponse);
                                }
                            }

                            await request.Render();
                            //await task;
                            return;
                        }
                    }

                    await SendError(result, webSocket, $"不支持的交互子对象'{jttp.Header.Type}'");
                    return;
                }
            }

            await SendError(result, webSocket, $"不支持的交互对象'{tps[0]}'");

        }
    }
}
