using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Threading.Tasks;
using dpz;

namespace Kernel.Wss.Requests {
    public class User : SessionRequest {

        public IRequestResult GetInfo() {

            long uid = Session["User_ID"].ToLong();
            if (uid <= 0) return Fail();

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosUsers]&[ID=='{uid}']}}");
                foreach (var item in row) {
                    if (item.Key != "Pwd")
                        JData[item.Key] = item.Value;
                }
            }

            return Success();
        }

        //获取GUID
        public IRequestResult Login() {
            if (!base.Verification) { return Fail("交互标识无效或已过期"); }

            string szName = JRequest.Data["Name"].Value;
            string szPwd = JRequest.Data["Pwd"].Value;

            long timeSession = Session["Session_Time"].ToLong();
            long timeLogin = Session["User_Login_Time"].ToLong();
            long timeNow = dpz.Time.Now.ToTimeStamp();

            if (timeNow - timeSession < 2) {
                return Fail("创建新的连接2秒内不允许进行用户登录");
            }

            if (timeNow - timeLogin < 2) {
                return Fail("操作过于频繁");
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosUsers]&[Name=='{szName}']}}");
                if (row.IsEmpty) {
                    return Fail("用户不存在");
                }
                string szSuperPwd = site.Config.Security.GetEncryptionPasswordString(szName, szPwd);
                string szUserPwd = row["Pwd"];
                if (szSuperPwd != szUserPwd) {
                    Session["User_Login_Time"] = "" + timeNow;
                    return Fail("密码错误");
                }

                Session["User_ID"] = row["ID"];
            }

            return Success();
        }

        //获取GUID
        public IRequestResult Repwd() {
            if (!base.Verification) { return Fail("交互标识无效或已过期"); }

            long userId = Session["User_ID"].ToLong();
            //string szName = JRequest.Data["Name"].Value;
            string oldPwd = JRequest.Data["OldPwd"].Value;
            string newPwd = JRequest.Data["NewPwd"].Value;
            string rePwd = JRequest.Data["RePwd"].Value;

            if (newPwd != rePwd) {
                return Fail("两次输入的密码不一致");
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                var row = dbc.GetGdmlOne($"@{{$[AosUsers]&[ID=='{userId}']}}");
                if (row.IsEmpty) {
                    return Fail("用户不存在");
                }

                //验证旧密码
                string oldSuperPwd = site.Config.Security.GetEncryptionPasswordString(row["Name"], oldPwd);
                string userPwd = row["Pwd"];
                if (oldSuperPwd != userPwd) {
                    //Session["User_Login_Time"] = "" + timeNow;
                    return Fail("密码错误");
                }

                //更新新密码
                string newSuperPwd = site.Config.Security.GetEncryptionPasswordString(row["Name"], newPwd);
                dbc.ExecGdml($"!{{$[AosUsers].[Pwd='{newSuperPwd}']&[ID=='{userId}']}}");

                //Session["User_ID"] = row["ID"];
            }

            return Success();
        }

        //获取GUID
        public IRequestResult Logout() {
            if (!base.Verification) { return Fail("交互标识无效或已过期"); }
            Session["User_ID"] = "0";
            return Success();
        }

    }
}
