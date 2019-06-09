using dpz.Jsons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Wss.RequestResults {
    public class Error : IRequestResult {

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// 错误信息
        /// </summary>
        public int Code { get; set; }

        public void SetResult(Jttp jttp) {
            //throw new NotImplementedException();
            jttp.Header.Status = "-1";
            jttp.Header.Error = "" + this.Code;
            jttp.Message = this.Message;

        }

    }
}
