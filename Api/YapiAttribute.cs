using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Api {

    /// <summary>
    /// 云谊通API专用识别标签
    /// </summary>
    public class YapiAttribute : site.Yattribute {

        public YapiAttribute(string desc = "") {
            this.Description = desc;
            this.LoginNeed = false;
        }

    }
}
