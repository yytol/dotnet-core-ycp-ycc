using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace site.Config {
    public class Url {

        private static string manage = "";
        private static string desktop = "";
        private static string manageConfig = "";
        private static string websocket = "";

        public static string Websocket {
            get {
                if (websocket == "") {

                    //取配置根下的 Database 部分
                    var orm = site.ConfigManager.Root.GetSection("Url");
                    websocket = orm.GetSection("Websocket").Value;

                }
                return websocket;
            }
        }

        public static string ManageConfig {
            get {
                if (manageConfig == "") {

                    //取配置根下的 Database 部分
                    var orm = site.ConfigManager.Root.GetSection("Url");
                    manageConfig = orm.GetSection("ManageConfig").Value;

                }
                return manageConfig;
            }
        }

        public static string Desktop {
            get {
                if (desktop == "") {

                    //取配置根下的 Database 部分
                    var orm = site.ConfigManager.Root.GetSection("Url");
                    desktop = orm.GetSection("Desktop").Value;

                }
                return desktop;
            }
        }
        public static string Manage {
            get {
                if (manage == "") {

                    //取配置根下的 Database 部分
                    var orm = site.ConfigManager.Root.GetSection("Url");
                    manage = orm.GetSection("Manage").Value;

                }
                return manage;
            }
        }

    }
}
