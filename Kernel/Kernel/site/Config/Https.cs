using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dpz;

namespace site.Config {
    public class Https {

        private static string enable = "";
        private static string port = "";
        private static string pfxPath = "";
        private static string pfxPwd = "";

        public static string PfxPassword {
            get {
                if (pfxPwd == "") {

                    //取配置根下的 Database 部分
                    var https = site.ConfigManager.Root.GetSection("Https");
                    var pfx = https.GetSection("Pfx");
                    pfxPwd = pfx.GetSection("Password").Value;

                }
                return pfxPwd;
            }
        }

        public static string PfxPath {
            get {
                if (pfxPath == "") {

                    //取配置根下的 Database 部分
                    var https = site.ConfigManager.Root.GetSection("Https");
                    var pfx = https.GetSection("Pfx");
                    pfxPath = pfx.GetSection("Path").Value;

                }
                return pfxPath;
            }
        }

        public static bool Enable {
            get {
                if (enable == "") {

                    //取配置根下的 Database 部分
                    var http = site.ConfigManager.Root.GetSection("Https");
                    enable = http.GetSection("Enable").Value.ToLower();

                }
                return enable == "yes";
            }
        }

        public static int Port {
            get {
                if (port == "") {

                    //取配置根下的 Database 部分
                    var http = site.ConfigManager.Root.GetSection("Https");
                    port = http.GetSection("Port").Value;

                }
                return port.ToInteger();
            }
        }

    }
}
