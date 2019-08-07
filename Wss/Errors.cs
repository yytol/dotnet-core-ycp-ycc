using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Wss {
    public class Errors {

        /// <summary>
        /// 头部错误
        /// </summary>
        public const int Header = 0x010000;
        public const int Header_Session = Header + 0x0100;
        public const int Header_Session_Invalid = Header_Session + 0x01;

    }
}
