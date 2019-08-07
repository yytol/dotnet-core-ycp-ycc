using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Reflection;
using System.Text;
using System.ComponentModel;
using dpz;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Redis;

namespace Kernel.Controllers {
    public class HelpController : site.Controller {

        [Description("API辅助接口，返回服务器上所有的API列表")]
        public IActionResult Index() {

            JResponse.Header.Time = "" + dpz.Time.Now.ToTimeStamp();

            //创建控制器类型列表
            List<Type> controllerTypes = new List<Type>();

            //加载程序集
            var assembly = Assembly.Load("Api");

            //获取程序集下所有的类，通过Linq筛选继承IController类的所有类型
            controllerTypes.AddRange(assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type)));

            JResponse.Data.Apis = new List<dpz.Dynamic>();

            //创建动态字符串，拼接json数据    注：现在json类型传递数据比较流行，比xml简洁
            //StringBuilder jsonBuilder = new StringBuilder();
            //jsonBuilder.Append("[");

            //遍历控制器类
            foreach (var controller in controllerTypes) {
                //dynamic dc = new dpz.Dynamic();
                string clrPath = "/" + controller.Name.Substring(0, controller.Name.Length - 10);
                //dcc.Description = (controller.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute) == null ? "" : (controller.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description;
                //dcc.Actions = new List<dpz.Dynamic>();
                //jsonBuilder.Append("{\"name\":\"");
                //jsonBuilder.Append(controller.Name);
                //jsonBuilder.Append("\",\"description\":\"");
                //jsonBuilder.Append();

                //获取对控制器的描述Description
                //jsonBuilder.Append("\",\"action\":[");

                //获取控制器下所有返回类型为ActionResult的方法，对MVC的权限控制只要限制所以的前后台交互请求就行，统一为ActionResult
                var actions = controller.GetMethods().Where(method => method.ReturnType.Name == "IActionResult");
                foreach (var action in actions) {

                    dynamic dc = new dpz.Dynamic();
                    JResponse.Data.Apis.Add(dc);

                    dc.Url = clrPath + "/" + action.Name;
                    dc.Description = (action.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute) == null ? "" : (action.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description;

                    //jsonBuilder.Append("{\"name\":\"");
                    //jsonBuilder.Append(action.Name);
                    //jsonBuilder.Append("\",\"discription\":\"");
                    //jsonBuilder.Append((action.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute) == null ? "" : (action.GetCustomAttribute(typeof(DescriptionAttribute)) as DescriptionAttribute).Description);    //获取对Action的描述
                    //jsonBuilder.Append("\"},");
                }
                //jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
                //jsonBuilder.Append("]},");
            }
            //jsonBuilder.Remove(jsonBuilder.Length - 1, 1);
            //jsonBuilder.Append("]");
            //JResponse.Data["Title"] = base.Title;
            return JttpContent(1);
            //return View();
        }

        public IActionResult Show() {
            return View();
        }

        //protected override void OnExecuting(ActionExecutingContext filterContext) {
        //    base.OnExecuting(filterContext);

        //    var redis = new RedisCache(new RedisCacheOptions() {
        //        Configuration = "",
        //        InstanceName = "SessionId"
        //    });
        //    redis.Set("abc", System.Text.Encoding.UTF8.GetBytes("123"), new DistributedCacheEntryOptions() {
        //        SlidingExpiration = TimeSpan.FromHours(1)
        //    });
        //}
    }
}