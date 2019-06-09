using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dpz.Jsons;

namespace Kernel.Wss.RequestResults {
    public class Success : IRequestResult {

        /// <summary>
        /// 获取或设置提示信息
        /// </summary>
        public string Message { get; set; }

        public void SetResult(Jttp jttp) {
            //throw new NotImplementedException();
            jttp.Header.Status = "1";
            jttp.Message = Message;
        }
    }
}
