using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace site.Config {
    public class Redis {

        private static string connectionString = "";

        public static string ConnectionString {
            get {
                if (connectionString == "") {

                    //取配置根下的 Database 部分
                    var redis = site.ConfigManager.Root.GetSection("Redis");

                    connectionString = $"{redis.GetSection("Address").Value}:{redis.GetSection("Port").Value},password={redis.GetSection("Password").Value}";
                }
                return connectionString;
            }
        }

    }
}
