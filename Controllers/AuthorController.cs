using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Kernel.Controllers {
    public class AuthorController : site.Controller {
        public IActionResult Index() {
            return View();
        }

        public IActionResult Authorize() {

            string res = "";

            //this.Session.WebOSHandle = this["Handle"];
            //this.Session.WebOSGuid = this["Guid"];
            string key = this["sn"];
            string redirect = HttpUtility.UrlDecode(this["redirect"], System.Text.Encoding.UTF8);

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetOne("@{$[AosAuthorize].[*]&[Code=='" + key + "']}");
                if (row.IsEmpty) {
                    return TextContent("授权失败:授权码不正确");
                }
                string code = "";
                do {
                    code = Guid.NewGuid().ToString().Replace("-", "");
                } while (!dbc.GetOne("@{$[AosAuthorizeCode].[*]&[Code=='" + code + "']}").IsEmpty);
                dbc.Exec("+{$[AosAuthorizeCode].[AuthID='" + row["ID"] + "'].[SessionID='" + this.Session.SessionID + "'].[Code='" + code + "'].[Token=''].[EffectiveTime='" + dpz.Time.New(DateTime.Now.AddMinutes(30)).ToTimeStamp() + "'].[Status='0']}");
                res = redirect + "?code=" + code;
            }

            Response.Redirect(res);

            return TextContent(res);
        }
    }
}