using dpz.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;

namespace Kernel.Wss {
    public abstract class JttpRequest {

        /// <summary>
        /// 获取相关的通讯套接字
        /// </summary>
        public site.WebSocket Socket { get; private set; } = null;

        /// <summary>
        /// 获取提交内容
        /// </summary>
        public dpz.Jsons.Jttp JRequest { get; private set; } = null;

        /// <summary>
        /// 获取提交内容中的数据
        /// </summary>
        public dynamic JReqData { get; private set; } = null;

        /// <summary>
        /// 获取数据接收结果
        /// </summary>
        public WebSocketReceiveResult ReceiveResult { get; private set; } = null;

        /// <summary>
        /// 获取呈送内容对象
        /// </summary>
        public dpz.Jsons.Jttp JResponse { get; private set; }

        /// <summary>
        /// 获取呈送内容头部对象
        /// </summary>
        public dpz.Jsons.JttpHeader JHeader { get; private set; }

        /// <summary>
        /// 获取额外推送的内容集合
        /// </summary>
        public List<Jttp> RenderJttps { get; private set; }

        /// <summary>
        /// 获取呈送内容数据对象
        /// </summary>
        public dynamic JData { get; private set; }

        /// <summary>
        /// 执行前事件
        /// </summary>
        /// <returns></returns>
        public virtual IRequestResult OnExecuting() { return null; }

        /// <summary>
        /// 执行后事件
        /// </summary>
        /// <returns></returns>
        public virtual IRequestResult OnExecuted() { return null; }

        public void Init(WebSocketReceiveResult result, site.WebSocket webSocket, Jttp jttp) {
            this.RenderJttps = new List<Jttp>();
            this.Socket = webSocket;
            this.JRequest = jttp;
            this.ReceiveResult = result;

            this.JResponse = new dpz.Jsons.Jttp();
            this.JHeader = this.JResponse.Header;
            this.JData = this.JResponse.Data;

            JReqData = JRequest.Data;
            JHeader.Type = JRequest.Header.Type;
            JHeader.SessionID = JRequest.Header.SessionID;

            //return this.OnInited();
        }

        protected virtual void OnBeforeRender() { }

        public async Task Render() {

            if (this.RenderJttps.Count > 0) {
                for (int i = 0; i < this.RenderJttps.Count; i++) {
                    await Render(this.RenderJttps[i]);
                }
            }

            JHeader.Time = "" + dpz.Time.Now.ToTimeStamp();
            this.OnBeforeRender();

            byte[] bs = System.Text.Encoding.UTF8.GetBytes(this.JResponse.ToJson());
            await this.Socket.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), this.ReceiveResult.MessageType, this.ReceiveResult.EndOfMessage, CancellationToken.None);
        }

        public async Task Render(Jttp jttp) {
            byte[] bs = System.Text.Encoding.UTF8.GetBytes(jttp.ToJson());
            await this.Socket.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), this.ReceiveResult.MessageType, this.ReceiveResult.EndOfMessage, CancellationToken.None);
        }

        protected RequestResults.Success Success(string msg = "") {
            return new RequestResults.Success() {
                Message = msg
            };
        }

        protected RequestResults.Fail Fail(string msg = "") {
            return new RequestResults.Fail() {
                Message = msg
            };
        }

        protected RequestResults.Error Error(string msg = "", int code = 0) {
            return new RequestResults.Error() {
                Message = msg,
                Code = code
            };
        }
        //发送Jttp数据
        //protected async Task Send(dpz.Dynamic data, int status = 1, string msg = "", int errCode = 0, string tp = "", string sid = "") {
        //    using (dpz.Jsons.Jttp jttp = new dpz.Jsons.Jttp()) {
        //        jttp.Header.Ver = "1.0";
        //        if (tp == "") {
        //            jttp.Header.Type = JRequest.Header.Type;
        //        } else {
        //            jttp.Header.Type = tp;
        //        }
        //        if (sid == "") {
        //            jttp.Header.SessionID = JRequest.Header.SessionID;
        //        } else {
        //            jttp.Header.SessionID = sid;
        //        }
        //        jttp.Header.Time = "" + dpz.Time.Now.ToTimeStamp();
        //        jttp.Header.Status = "" + status;
        //        jttp.Header.Error = "" + errCode;
        //        if (msg != "") jttp.Message = msg;

        //        if (data != null) {
        //            foreach (var item in data) {
        //                jttp.Data[item.Key] = item.Value;
        //            }
        //        }

        //        byte[] bs = System.Text.Encoding.UTF8.GetBytes(jttp.ToJson());
        //        await this.Socket.Socket.SendAsync(new ArraySegment<byte>(bs, 0, bs.Length), this.ReceiveResult.MessageType, this.ReceiveResult.EndOfMessage, CancellationToken.None);
        //    }
        //}

    }
}
