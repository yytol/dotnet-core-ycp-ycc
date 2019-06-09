using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace site {
    public class ConfigManager {

        private static IConfigurationRoot root = null;

        public static IConfigurationRoot Root {
            get {
                if (root == null) {
                    //添加 json 文件路径
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("site_config.json");
                    //创建配置根对象
                    //var configurationRoot = builder.Build();
                    root = builder.Build();
                }
                return root;
            }
        }

    }
}
