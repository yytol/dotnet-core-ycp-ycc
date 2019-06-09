using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Kernel {
    public class Program {
        public static void Main(string[] args) {
            CreateWebHostBuilder(args)
                .Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) {
            var builder = WebHost.CreateDefaultBuilder(args);

            if (site.Config.Https.Enable) {
                builder.UseKestrel(options => {
                    options.Listen(IPAddress.Any, site.Config.Https.Port, listenOptions => {
                        //填入之前iis中生成的pfx文件路径和指定的密码　　　　　　　　　　　　
                        listenOptions.UseHttps(site.Config.Https.PfxPath, site.Config.Https.PfxPassword);
                    });
                });
            }

            if (site.Config.Http.Enable) {
                builder.UseKestrel(options => {
                    options.Listen(IPAddress.Any, site.Config.Http.Port);
                });
            }

            return builder.UseStartup<Startup>();
        }

        //public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
        //    WebHost.CreateDefaultBuilder(args)
        //        //.UseUrls(new string[] { "https://127.0.0.1:8443", "https://api.lywos.com:8443" })
        //        //设置Kestrel服务器
        //        .UseKestrel(options => {
        //            options.Listen(IPAddress.Any, 443, listenOptions => {
        //                //填入之前iis中生成的pfx文件路径和指定的密码　　　　　　　　　　　　
        //                listenOptions.UseHttps("/web/ssl/lywos.com.pfx", "g8rlr1QC");
        //            });
        //        })
        //        .UseKestrel(options => {
        //            options.Listen(IPAddress.Any, 80);
        //        })
        //        .UseStartup<Startup>();
    }
}
