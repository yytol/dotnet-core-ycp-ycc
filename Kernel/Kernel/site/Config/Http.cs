using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dpz;

namespace site.Config {
    public class Http {

        private static string enable = "";
        private static string port = "";

        public static bool Enable {
            get {
                if (enable == "") {

                    //取配置根下的 Database 部分
                    var http = site.ConfigManager.Root.GetSection("Http");
                    enable = http.GetSection("Enable").Value.ToLower();

                }
                return enable == "yes";
            }
        }

        public static int Port {
            get {
                if (port == "") {

                    //取配置根下的 Database 部分
                    var http = site.ConfigManager.Root.GetSection("Http");
                    port = http.GetSection("Port").Value;

                }
                return port.ToInteger();
            }
        }

    }
}
