using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Controllers {

    /// <summary>
    /// 云谊通API专用识别标签
    /// </summary>
    public class YactionAttribute : ActionFilterAttribute {

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public bool LoginNeed { get; set; }

        /// <summary>
        /// 获取或设置描述
        /// </summary>
        public string Title { get; set; }

        public YactionAttribute(string title = "") {
            this.Title = title;
            this.Description = "";
            this.LoginNeed = false;
        }

        public override void OnResultExecuting(ResultExecutingContext context) {
            var ctlr = context.Controller as Controller;
            if (ctlr == null) throw new Exception("未能获取控制器");
            base.OnResultExecuting(context);

            if (this.Title != "") ctlr.ViewData["Title"] = this.Title;
            if (this.Description != "") ctlr.ViewData["Description"] = this.Description;
        }

    }
}
