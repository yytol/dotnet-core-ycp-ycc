using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dpz.Jsons;

namespace Kernel.Wss.RequestResults {
    public class Fail : IRequestResult {

        /// <summary>
        /// 错误信息
        /// </summary>
        public string Message { get; set; }

        public void SetResult(Jttp jttp) {
            //throw new NotImplementedException();
            jttp.Header.Status = "0";
            jttp.Message = this.Message;
        }
    }
}
