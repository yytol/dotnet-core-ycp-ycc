using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using dpz;
using Microsoft.Extensions.DependencyModel;
using System.ComponentModel;
using Microsoft.AspNetCore.Hosting;

namespace Kernel.Api {
    [Route("Api/[controller]")]
    [ApiController]
    public class InfoController : ControllerBase {

        /// <summary>
        /// 获取Linux服务器资源信息
        /// </summary>
        private const string ETH0_CONFIG_FILE_PATH = @"/sys/class/net/eth0/address";
        private const string WLAN0_CONFIG_FILE_PATH = @"/sys/class/net/wlan0/address";

        private IHostingEnvironment host;

        public InfoController(IHostingEnvironment hostingEnvironment) {
            host = hostingEnvironment;
        }

        //填充内核信息
        private void FillKernel(dpz.Dynamic obj) {
            dynamic dyc = new dpz.Dynamic();
            dyc.Name = "云谊通核心控制器";
            dyc.Description = "云谊通核心控制器(Yunyitong Core Controller，简称Ycc)是云谊通云应用协作平台的核心组件，是一套基于.Net Core技术研发、包含H5、Web API、Websocket通讯为基础的综合性数据管理系统，为平台提供最基础的用户授权、数据认证和通讯保障。";
            dyc.Version = "5.0.1901.1";
            obj["Kernel"] = dyc;
        }

        //填充内核信息
        private void FillUpdate(dpz.Dynamic obj) {
            var logs = new List<dpz.Dynamic>();
            obj["Update"] = logs;

            dynamic v1 = new dpz.Dynamic();
            v1.Version = "5.0.1901.1";
            List<string> lsV1 = new List<string>();
            v1.Logs = lsV1;
            lsV1.Add("核心技术升级为基于.Net Core 2.2框架，独立进程运行，响应效率更高");
            lsV1.Add("支持跨平台部署，可支持将平台部署至Windows、Linux或MacOS");
            lsV1.Add("核心通讯升级为WebSocket，网络延迟更低，平台响应速度更快");
            logs.Add(v1);
        }

        //填充内核信息
        private void FillEnvironment(dpz.Dynamic obj) {
            var dc = DependencyContext.Default;
            //Console.WriteLine($"Framework: {dc.Target.Framework}");

            dynamic envir = new dpz.Dynamic();
            obj["Environment"] = envir;
            envir.OSFullName = RuntimeInformation.OSDescription;
            envir.OSArchitecture = RuntimeInformation.OSArchitecture;
            envir.ProcessArchitecture = RuntimeInformation.ProcessArchitecture;
            envir.Framework = dc.Target.Framework;
            envir.Time = dpz.Time.Now.ToTimeStamp();
            envir.WorkPath = Directory.GetCurrentDirectory();
            envir.IPAddress = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();
            //envir.RootPath = IServer.MapPath;

            //判断操作系统
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {
                envir.System = "Windows";
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                envir.System = "Linux";
            } else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) {
                envir.System = "MacOS";
            } else {
                envir.System = "Unknow";
            }
            //envir.SystemVersion = RuntimeEnvironment.GetSystemVersion();

            //NetworkInfo networkInfo = new NetworkInfo();
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {
                string eth0 = dpz.IO.UTF8File.ReadAllText(ETH0_CONFIG_FILE_PATH);
                int idx = eth0.IndexOf("\n");
                if (idx > 0) eth0 = eth0.Substring(0, idx);
                envir.EthernetMAC = eth0;

                string wlan0 = dpz.IO.UTF8File.ReadAllText(WLAN0_CONFIG_FILE_PATH);
                idx = wlan0.IndexOf("\n");
                if (idx > 0) wlan0 = wlan0.Substring(0, idx);
                envir.WirelessMAC = wlan0;

                string deviceId = "eth0+" + eth0 + "+wlan0+" + wlan0;
                envir.DeviceId = deviceId.GetMD5().ToLower();
            }

            var platforms = new List<dpz.Dynamic>();
            envir.Platforms = platforms;
            dc.RuntimeLibraries
            .Where(x => x.Name.Contains("Microsoft.NETCore.App"))
            .ToList()
            .ForEach(x => {
                //Console.WriteLine($"{x.Name} {x.Version}");
                dynamic platform = new dpz.Dynamic();
                platform.Name = x.Name;
                platform.Version = x.Version;
                platforms.Add(platform);
            });

        }

        //填充内核信息
        private void FillDebug(dpz.Dynamic obj) {

            dynamic cfg = new dpz.Dynamic();
            obj["Config"] = cfg;

            dynamic redis = new dpz.Dynamic();
            cfg["Redis"] = redis;
            redis.ConnectionString = site.Config.Redis.ConnectionString;

        }

        //[HttpGet("Kernel")]
        //[Yapi("获取核心信息")]
        //public string Kernel() {
        //    dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();
        //    FillKernel(res.Data);
        //    return res.ToJson();
        //}

        [HttpGet("Update")]
        [Yapi("获取更新日志")]
        public string Update() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();
            FillUpdate(res.Data);
            return res.ToJson();
        }

        [HttpGet("Environment")]
        [Yapi("获取运行环境信息")]
        public string Environment() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();
            FillEnvironment(res.Data);
            return res.ToJson();
        }

        [HttpGet("Debug")]
        [Yapi("获取运行环境信息")]
        public string Debug() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();
            FillDebug(res.Data);
            return res.ToJson();
        }

        [HttpGet]
        [Yapi("获取所有与本程序相关的信息")]
        public string Get() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();

            //FillKernel(res.Data);

            //FillUpdate(res.Data);

            FillEnvironment(res.Data);
            res.Data.Environment.RootPath = host.ContentRootPath;

            return res.ToJson();
        }

    }
}