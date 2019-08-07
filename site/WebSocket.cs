using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace site {
    public class WebSocket : IDisposable {

        public delegate void RequestHandler(WebSocketReceiveResult result, System.Net.WebSockets.WebSocket webSocket, dpz.Jsons.Jttp jttp);

        /// <summary>
        /// 获取当前工作状态
        /// </summary>
        public bool IsWorking { get; private set; } = false;

        /// <summary>
        /// 获取Http相关的上下文
        /// </summary>
        public HttpContext Context { get; private set; } = null;

        /// <summary>
        /// 唯一标识符
        /// </summary>
        public string Guid { get; internal set; } = "";

        /// <summary>
        /// 获取相关的通讯套接字
        /// </summary>
        public System.Net.WebSockets.WebSocket Socket { get; private set; } = null;

        /// <summary>
        /// 获取相关的处理器
        /// </summary>
        public IWebsocketProcessor Processor { get; private set; } = null;

        //private Thread thread;//工作线程

        public WebSocket(HttpContext context, System.Net.WebSockets.WebSocket webSocket, IWebsocketProcessor processor) {
            this.Context = context;
            this.Socket = webSocket;
            this.Processor = processor;
        }

        public void Dispose() {
            //throw new NotImplementedException();
        }

        //获取GUID
        private async Task SendError(WebSocketReceiveResult result, string msg, int code = 0, string tp = "") {
            using (dpz.Jsons.Jttp jttp = new dpz.Jsons.Jttp()) {
                jttp.Header.Ver = "1.0";
                jttp.Header.Type = tp;
                jttp.Header.Time = "" + dpz.Time.Now.ToTimeStamp();
                jttp.Header.Status = "-1";
                jttp.Header.Error = "" + code;
                jttp.Message = msg;

                byte[] bs = System.Text.Encoding.UTF8.GetBytes(jttp.ToJson());
                await this.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), result.MessageType, result.EndOfMessage, CancellationToken.None);
            }
        }

        public async Task Start() {
            var buffer = new byte[1024 * 4];
            WebSocketReceiveResult result = await this.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            this.IsWorking = true;

            while (!result.CloseStatus.HasValue) {
                //个人觉得实际上可以将下面的webSocket.SendAsync方法放到一个Task里面，来启动另一个线程然后在内部不断循环来处理发送数据的任务，避免和下面的webSocket.ReceiveAsync轮流执行，从而提高效率
                int len = result.Count;
                byte[] bs = new byte[len];
                Array.Copy(buffer, 0, bs, 0, len);
                string res = System.Text.Encoding.UTF8.GetString(bs);
                try {
                    using (dpz.Jsons.Jttp jttp = dpz.Jsons.Jttp.Parse(res)) {
                        if (jttp.Header.Type == "") {
                            await SendError(result, "未发现交互类型"); break;
                        } else {
                            await this.Processor.Run(result, this, jttp);
                        }
                    }
                } catch (Exception ex) {
                    await SendError(result, "未知错误=>" + ex.ToString().Replace("\n", "|").Replace("\r", "") + "=>Json:" + res);
                }
                //byte[] bs = System.Text.Encoding.UTF8.GetBytes("收到数据:" + res);

                result = await this.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }
            await this.Socket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }

    }
}
