using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;

namespace site {
    public class RazorPage : Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic> {

        /// <summary>
        /// 获取或设置网页标题
        /// </summary>
        public string Title { get { return ViewContext.ViewData["Title"] as string; } set { ViewContext.ViewData["Title"] = value; } }

        public RazorPage() {
            //var controllers = assembly.GetTypes().Where(type => typeof(Controller).IsAssignableFrom(type));

            //foreach (var controller in controllers) {
            //    if (controller.Name == controllerName) {

            //        var acts = controller.GetMethods().Where(method => method.Name == actionName);
            //        foreach (var act in acts) {
            //            var attrs = act.GetCustomAttributes(true);
            //            foreach (var attr in attrs) {
            //                if (attr.GetType().FullName == "Kernel.Controllers.YactionAttribute") {
            //                    var yact = attr as Kernel.Controllers.YactionAttribute;
            //                    ViewContext.ViewData["Title"] = yact.Title;
            //                    break;
            //                }
            //            }
            //        }

            //        break;
            //    }
            //}
        } 

        protected override IHtmlContent RenderBody() {

            //var context = ViewContext;
            //if (context == null) throw new Exception("无法获取控制上下文");

            //var controllerName = context.RouteData.Values["Controller"].ToString();
            //var controllerClassName = controllerName + "Controller";
            //var actionName = context.RouteData.Values["Action"].ToString();

            //var assembly = Assembly.GetExecutingAssembly();
            //ViewData["Debug"] = assembly.FullName;

            return base.RenderBody();
        }

        public override Task ExecuteAsync() {
            throw new NotImplementedException();
        }
    }
}
