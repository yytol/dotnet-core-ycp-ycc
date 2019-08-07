using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site {
    public class Yattribute : Attribute {

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public bool LoginNeed { get; set; }

    }
}
