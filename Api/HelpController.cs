using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Kernel.Api {
    [Route("Api/[controller]")]
    [ApiController]
    public class HelpController : ControllerBase {

        [HttpGet]
        [Yapi("API辅助接口，返回服务器上所有的API列表")]
        public string Get() {
            dpz.Jsons.Jttp res = new dpz.Jsons.Jttp();

            res.Header.Time = "" + dpz.Time.Now.ToTimeStamp();

            //创建控制器类型列表
            List<Type> controllerTypes = new List<Type>();

            //加载程序集
            var assembly = Assembly.GetExecutingAssembly();

            //获取程序集下所有的类，通过Linq筛选继承IController类的所有类型
            controllerTypes.AddRange(assembly.GetTypes().Where(type => typeof(ControllerBase).IsAssignableFrom(type)));

            res.Data.Apis = new List<dpz.Dynamic>();

            //遍历控制器类
            foreach (var controller in controllerTypes) {
                //dynamic dc = new dpz.Dynamic();

                string clrName = controller.Name;
                if (clrName.EndsWith("Controller")) clrName = clrName.Substring(0, controller.Name.Length - 10);
                string clrPath = "/" + controller.Name.Substring(0, controller.Name.Length - 10);
                var actions = controller.GetMethods().Where(method => method.IsPublic == true);

                var route = controller.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;
                if (route != null) {
                    clrPath = "/" + route.Template.Replace("[controller]", clrName);
                    //dc.Route = route.Name;
                }

                foreach (var action in actions) {

                    var yapi = action.GetCustomAttribute(typeof(YapiAttribute)) as YapiAttribute;

                    if (yapi != null) {
                        dynamic dc = new dpz.Dynamic();
                        res.Data.Apis.Add(dc);

                        dc.Url = clrPath + "/" + action.Name;
                        dc.LoginNeed = yapi.LoginNeed;
                        dc.Controller = controller.FullName;
                        dc.Description = yapi.Description;

                    }
                }
            }
            return res.ToJson();

            //return obj.ToJson();
        }

    }
}