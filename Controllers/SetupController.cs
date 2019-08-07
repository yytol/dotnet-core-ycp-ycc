using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using dpz;

namespace Kernel.Controllers {

    public class SetupController : site.RedisJsonController {

        [Yaction("安装平台数据库 - 云谊通核心控制器")]
        public IActionResult Index() {
            return View();
        }

        [Yaction("安装平台数据表 - 云谊通核心控制器")]
        public IActionResult Step2() {
            return View();
        }

        [Yaction("安装管理数据库及授权 - 云谊通核心控制器")]
        public IActionResult Step3() {
            return View();
        }

        [Yaction("安装管理数据表 - 云谊通核心控制器")]
        public IActionResult Step4() {
            return View();
        }

        [Yaction("初始化管理应用 - 云谊通核心控制器")]
        public IActionResult Step5() {
            return View();
        }

        [Yaction("初始化超级管理员 - 云谊通核心控制器")]
        public IActionResult Step6() {
            return View();
        }

        [Yaction("初始化超级管理员应用关联 - 云谊通核心控制器")]
        public IActionResult Step7() {
            return View();
        }

        public IActionResult CreateAos() {

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Defend)) {
                //var row = dbc.GetOne("select * from sys.databases where [name] = 'Aos'");
                if (dbc.CheckDatabase("Aos")) {
                    return JsonContent(0, "数据库已经存在");
                }

                dbc.CreateDatabase("Aos");
            }

            return JsonContent(1);
        }

        public ActionResult CreateAosTable() {

            //string plmName = this["platform"];
            string tabName = this["table"];

            if (tabName != "AosObjects") {
                using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                    if (!dbc.CheckTable("AosObjects")) {
                        return JsonContent(0, $"请在安装{tabName}表前先安装AosObjects表");
                    }
                }
            }

            string tabVersion = "";
            string xmlSetting = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Setting.xml");
            using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlSetting)) {
                var xmlDB = xml["database"];
                var xmlAos = xmlDB.GetNodeByAttrValue("name", "Aos", false);
                var xmlTable = xmlAos.GetNodeByAttrValue("name", tabName, false);
                tabVersion = xmlTable.Attr["version"];
            }

            string xmlString = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Aos/{tabName}.xml");

            if (xmlString == "") {
                return JsonContent(0, "配置获取失败,请检查表名称是否存在");
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {

                //string sql = "";

                using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlString)) {
                    var xmlTable = xml["table"];

                    //判断表是否存在
                    if (!dbc.CheckTable(tabName)) {

                        List<dpz.Gdbc.TableFieldDefine> fields = new List<dpz.Gdbc.TableFieldDefine>();

                        foreach (var xmlField in xmlTable.Nodes) {
                            if (xmlField.Name.ToLower() == "field") {
                                string fieldName = xmlField.Attr["name"];
                                var xmlData = xmlField["data"];
                                string fieldDataType = xmlData.Attr["type"].ToLower();
                                int fieldDataSize = xmlData.Attr["size"].ToInteger();
                                int fieldDataFloat = xmlData.Attr["float"].ToInteger();

                                var field = new dpz.Gdbc.TableFieldDefine();
                                field.Name = fieldName;
                                field.Type = fieldDataType;
                                field.Size = fieldDataSize;
                                field.Float = fieldDataFloat;
                                fields.Add(field);
                            }
                        }

                        dbc.CreateTable(tabName, fields);
                    } else {
                        //更新字段
                        foreach (var xmlField in xmlTable.Nodes) {
                            if (xmlField.Name.ToLower() == "field") {
                                string fieldName = xmlField.Attr["name"];
                                var xmlData = xmlField["data"];
                                string fieldDataType = xmlData.Attr["type"].ToLower();
                                int fieldDataSize = xmlData.Attr["size"].ToInteger();
                                int fieldDataFloat = xmlData.Attr["float"].ToInteger();

                                dpz.Gdbc.TableFieldDefine fieldDefine = new dpz.Gdbc.TableFieldDefine() {
                                    Name = fieldName,
                                    Type = fieldDataType,
                                    Size = fieldDataSize,
                                    Float = fieldDataFloat
                                };

                                if (!dbc.CheckTableFiled(tabName, fieldName)) {
                                    dbc.AddTableFiled(tabName, fieldDefine);
                                } else {
                                    dbc.UpdateTableFiled(tabName, fieldName, fieldDefine);
                                }
                            }
                        }
                    }

                    //更新表格结构信息
                    if (dbc.GetGdmlOne($"@{{$[AosObjects]&[Name=='{tabName}']}}").IsEmpty) {
                        dbc.ExecGdml($"+{{$[AosObjects].[Name='{tabName}'].[Version='{tabVersion}']}}");
                    } else {
                        dbc.ExecGdml($"!{{$[AosObjects].[Version='{tabVersion}']&[Name=='{tabName}']}}");
                    }

                    JResponse["Version"] = tabVersion;
                }
            }

            return JsonContent(1);
        }

        public ActionResult CreateManage() {

            long authId = 0;
            long desktopId = 0;

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosDesktops")) {
                    return JsonContent(0, $"请先安装AosDesktops表");
                }

                //添加默认桌面
                if (dbc.GetGdmlOne($"@{{$[AosDesktops]&[Host=='*']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosDesktops].[Name='Ycp'].[Text='云谊通云平台'].[Path=''].[Host='*'].[ScriptEntrance='/js/load.js'].[UrlEntrance='{site.Config.Url.Desktop}'].[Description='专业云应用协作平台']}}");
                }

                desktopId = dbc.GetGdmlOne($"@{{$[AosDesktops]&[Host=='*']}}")["ID"].ToLong();
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosAuthorize")) {
                    return JsonContent(0, $"请先安装AosAuthorize表");
                }

                //添加授权信息
                if (dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}").IsEmpty) {

                    string code = "";
                    string securityKey = Guid.NewGuid().ToString().Replace("-", "");

                    do {
                        code = Guid.NewGuid().ToString();
                    } while (!dbc.GetGdmlOne($"@{{$[AosAuthorize]&[Code=='{code}']}}").IsEmpty);

                    JResponse["Code"] = code;
                    JResponse["SecurityKey"] = securityKey;

                    dbc.ExecGdml($"+{{$[AosAuthorize].[Name='系统管理'].[Code='{code}'].[SecurityKey='{securityKey}'].[Lv='0'].[DBType='SqlServer'].[DBSign='Manage'].[DBIP=''].[DBPort='0'].[DBUser=''].[DBPwd=''].[ScriptEntrance='/js/load.js'].[UrlEntrance='{site.Config.Url.Manage}']}}");
                }

                authId = dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}")["ID"].ToLong();
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosDesktopAuthorizes")) {
                    return JsonContent(0, $"请先安装AosDesktopAuthorizes表");
                }

                //添加桌面授权兼容信息
                var row = dbc.GetGdmlOne($"@{{$[AosDesktopAuthorizes]&[DesktopID=='{desktopId}'&&AuthID=='{authId}']}}");
                if (row.IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosDesktopAuthorizes].[AuthID='{authId}'].[DesktopID='{desktopId}'].[Compatibility='1']}}");
                } else {
                    if (row["Compatibility"].ToInteger() <= 0) {
                        dbc.ExecGdml($"!{{$[AosDesktopAuthorizes].[Compatibility='1']&[ID=='{row["ID"]}']}}");
                    }
                }
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Defend)) {

                //检测数据库，不存在则创建
                if (!dbc.CheckDatabase("Aos_Manage")) dbc.CreateDatabase("Aos_Manage");
            }

            return JsonContent(1);
        }

        public ActionResult CreateManageTable() {

            string plmName = this["platform"];
            string tabName = this["table"];

            if (tabName != "SystemObjects") {
                using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Manage)) {
                    if (!dbc.CheckTable("SystemObjects")) {
                        return JsonContent(0, $"请先安装SystemObjects表");
                    }
                }
            }

            string tabVersion = "";
            string xmlSetting = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Setting.xml");
            using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlSetting)) {
                var xmlDB = xml["database"];
                var xmlAos = xmlDB.GetNodeByAttrValue("name", plmName, false);
                var xmlTable = xmlAos.GetNodeByAttrValue("name", tabName, false);
                tabVersion = xmlTable.Attr["version"];
            }

            string xmlString = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/{plmName}/{tabName}.xml");

            if (xmlString == "") {
                return JsonContent(0, "配置获取失败,请检查表名称是否存在");
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Manage)) {

                string sql = "";

                using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlString)) {
                    var xmlTable = xml["table"];

                    //判断表是否存在
                    //if (!dbc.GetOne($"Select * From sysObjects Where Name ='{tabName}' And Type In ('S','U')").HasData) {
                    if (!dbc.CheckTable(tabName)) {
                        //添加表
                        List<dpz.Gdbc.TableFieldDefine> fields = new List<dpz.Gdbc.TableFieldDefine>();

                        foreach (var xmlField in xmlTable.Nodes) {
                            if (xmlField.Name.ToLower() == "field") {
                                string fieldName = xmlField.Attr["name"];
                                var xmlData = xmlField["data"];
                                string fieldDataType = xmlData.Attr["type"].ToLower();
                                int fieldDataSize = xmlData.Attr["size"].ToInteger();
                                int fieldDataFloat = xmlData.Attr["float"].ToInteger();

                                var field = new dpz.Gdbc.TableFieldDefine();
                                field.Name = fieldName;
                                field.Type = fieldDataType;
                                field.Size = fieldDataSize;
                                field.Float = fieldDataFloat;
                                fields.Add(field);
                            }
                        }

                        dbc.CreateTable(tabName, fields);
                    } else {
                        //更新字段
                        foreach (var xmlField in xmlTable.Nodes) {
                            if (xmlField.Name.ToLower() == "field") {
                                string fieldName = xmlField.Attr["name"];
                                var xmlData = xmlField["data"];
                                string fieldDataType = xmlData.Attr["type"].ToLower();
                                int fieldDataSize = xmlData.Attr["size"].ToInteger();
                                int fieldDataFloat = xmlData.Attr["float"].ToInteger();

                                dpz.Gdbc.TableFieldDefine fieldDefine = new dpz.Gdbc.TableFieldDefine() {
                                    Name = fieldName,
                                    Type = fieldDataType,
                                    Size = fieldDataSize,
                                    Float = fieldDataFloat
                                };

                                if (!dbc.CheckTableFiled(tabName, fieldName)) {
                                    dbc.AddTableFiled(tabName, fieldDefine);
                                } else {
                                    dbc.UpdateTableFiled(tabName, fieldName, fieldDefine);
                                }
                            }
                        }
                    }

                    //更新表格结构信息
                    if (dbc.GetGdmlOne($"@{{$[SystemObjects]&[Name=='{tabName}'&&Type=='Table']}}").IsEmpty) {

                        string guid = "";
                        do {
                            guid = Guid.NewGuid().ToString();
                        } while (!dbc.GetGdmlOne($"@{{$[SystemObjects]&[Guid=='{guid}']}}").IsEmpty);

                        dbc.ExecGdml($"+{{$[SystemObjects].[Name='{tabName}'].[Type='Table'].[Version='{tabVersion}'].[Guid='{guid}']}}");
                    } else {
                        dbc.ExecGdml($"!{{$[SystemObjects].[Version='{tabVersion}']&[Name=='{tabName}'&&Type=='Table']}}");
                    }

                    JResponse["Version"] = tabVersion;
                }
            }

            return JsonContent(1);
        }

        public ActionResult CreateManageApp() {

            long authId = 0;
            string sign = this["sign"];

            string ip = System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
                .Select(p => p.GetIPProperties())
                .SelectMany(p => p.UnicastAddresses)
                .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
                .FirstOrDefault()?.Address.ToString();

            string manageUrl = site.Config.Url.Manage.Replace("${IP}", ip);
            string appConfigUrl = site.Config.Url.ManageConfig.Replace("${IP}", ip);
            string appXmlString = dpz.Net.Http.GetUTF8(appConfigUrl);

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosAuthorize")) {
                    return JsonContent(0, $"请先安装AosAuthorize表");
                }

                var row = dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}");
                if (row.IsEmpty) {
                    return JsonContent(0, "请先添加管理授权");
                }
                authId = row["ID"].ToLong();
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosApps")) {
                    return JsonContent(0, $"请先安装AosApps表");
                }

                using (dpz.Data.Xml xml = new dpz.Data.Xml(appXmlString)) {
                    var xmlEntity = xml["entity"];
                    var xmlApps = xmlEntity["apps"];
                    string appsSign = xmlApps.Attr["sign"];
                    if (appsSign != "") { appsSign += "."; }
                    foreach (var xmlApp in xmlApps.Nodes) {
                        if (xmlApp.Name == "app") {
                            string appSign = appsSign + xmlApp.Attr["sign"];

                            if (appSign == sign) {
                                string appTitle = xmlApp.Attr["title"];
                                string appVer = xmlApp.Attr["version"];
                                string appPath = xmlApp.Attr["path"];
                                var rowVer = "";

                                var rowApp = dbc.GetGdmlOne($"@{{$[AosApps]&[AuthID=='{authId}'&&Name=='{appSign}']}}");
                                if (!rowApp.IsEmpty) { rowVer = rowApp["Version"]; }

                                if (rowVer == "") {
                                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='{appSign}'].[Text='{appTitle}'].[Path='{appPath}'].[Version='{appVer}'].[OnStore='0'].[IsDesktop='0'].[Description=''].[CatalogID='0']}}");
                                } else if (appVer != rowVer) {
                                    dbc.ExecGdml($"!{{$[AosApps].[Text='{appTitle}'].[Path='{appPath}'].[Version='{appVer}']&[ID=='{rowApp["ID"]}']}}");
                                }

                                return JsonContent(1);
                            }

                        }
                    }


                }

            }

            return JsonContent(0, $"未找到应用配置信息");
        }

        public ActionResult CreateApp() {

            long authId = 0;

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosAuthorize")) {
                    return JsonContent(0, $"请先安装AosAuthorize表");
                }

                var row = dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}");
                if (row.IsEmpty) {
                    return JsonContent(0, "请先添加管理授权");
                }
                authId = row["ID"].ToLong();
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosApps")) {
                    return JsonContent(0, $"请先安装AosApps表");
                }

                //添加APP管理应用
                if (dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='AosApps']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='AosApps'].[Text='应用管理'].[Path='{site.Config.Url.Manage}/App/AosApps/Index/'].[OnStore='0'].[IsDesktop='0'].[Description='管理所有的应用'].[CatalogID='0']}}");
                }

                //添加用户管理应用
                if (dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='AosUsers']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='AosUsers'].[Text='用户管理'].[Path='{site.Config.Url.Manage}/App/AosUsers/Index/'].[OnStore='0'].[IsDesktop='0'].[Description='管理所有的平台用户'].[CatalogID='0']}}");
                }

                //添加授权管理应用
                if (dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='AosAuthorize']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='AosAuthorize'].[Text='授权管理'].[Path='{site.Config.Url.Manage}/App/Authorize/Index/'].[OnStore='0'].[IsDesktop='0'].[Description='管理所有的平台授权'].[CatalogID='0']}}");
                }

                //添加用户APP管理应用
                if (dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='AosUserApps']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='AosUserApps'].[Text='用户应用管理'].[Path='{site.Config.Url.Manage}/App/AosUserApps/Index/'].[OnStore='0'].[IsDesktop='0'].[Description='管理所有的平台用户关联应用'].[CatalogID='0']}}");
                }

                //添加用户APP管理应用
                if (dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='AosUserAuthorize']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosApps].[AuthID='{authId}'].[Name='AosUserAuthorize'].[Text='用户授权管理'].[Path='{site.Config.Url.Manage}/App/AosUserAuthorize/Index/'].[OnStore='0'].[IsDesktop='0'].[Description='管理所有的平台用户关联授权'].[CatalogID='0']}}");
                }
            }

            return JsonContent(1);
        }

        public ActionResult CreateRoot() {

            long authId = 0;

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosAuthorize")) {
                    return JsonContent(0, $"请先安装AosAuthorize表");
                }

                if (!dbc.CheckTable("AosUsers")) {
                    return JsonContent(0, $"请先安装AosUsers表");
                }

                if (!dbc.CheckTable("AosApps")) {
                    return JsonContent(0, $"请先安装AosApps表");
                }

                if (!dbc.CheckTable("AosUserApps")) {
                    return JsonContent(0, $"请先安装AosUserApps表");
                }

                if (!dbc.CheckTable("AosUserAuthorize")) {
                    return JsonContent(0, $"请先安装AosUserAuthorize表");
                }

                var row = dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}");
                if (row.IsEmpty) {
                    return JsonContent(0, "请先添加管理授权");
                }

                authId = row["ID"].ToLong();
            }

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {

                //添加根用户
                var rowUser = dbc.GetGdmlOne($"@{{$[AosUsers]&[Name=='root']}}");
                if (rowUser.IsEmpty) {
                    string pwd = site.Config.Security.GetEncryptionPasswordString("root", "123456");
                    // "$" + YString.New("000000root" + web.Config.Password_Key).MD5;
                    dbc.ExecGdml($"+{{$[AosUsers].[Name='root'].[Pwd='{pwd}'].[Nick='超级管理员'].[Image=''].[Desktop='/App/Ydm2Desktop'].[Background=''].[Limit=''].[AuthorizeID='{authId}']}}");
                    rowUser = dbc.GetGdmlOne($"@{{$[AosUsers]&[Name=='root']}}");
                }

                //添加根用户授权
                if (dbc.GetGdmlOne($"@{{$[AosUserAuthorize]&[UserID=='{rowUser["ID"]}'&&AuthID=='{authId}']}}").IsEmpty) {
                    dbc.ExecGdml($"+{{$[AosUserAuthorize].[UserID='{rowUser["ID"]}'].[AuthID='{authId}'].[Limit='']}}");
                }

                //添加根用户关联APP
                //string[] appNeeds = new string[] { "AosApps", "AosUserApps", "AosUserAuthorize", "AosUsers", "AosAuthorize" };
                //for (int i = 0; i < appNeeds.Length; i++) {
                //    var rowApp = dbc.GetGdmlOne($"@{{$[AosApps]&[Name=='{appNeeds[i]}']}}");
                //    if (!rowApp.IsEmpty) {
                //        if (dbc.GetGdmlOne($"@{{$[AosUserApps]&[UserID=='{rowUser["ID"]}'&&AppID=='{rowApp["ID"]}']}}").IsEmpty)
                //            dbc.ExecGdml($"+{{$[AosUserApps].[UserID='{rowUser["ID"]}'].[AuthID='0'].[AppID='{rowApp["ID"]}'].[Limit='']}}");
                //    }
                //}


            }

            return JsonContent(1);
        }

        public ActionResult CreateRootUserApp() {

            long authId = 0;
            long appId = this["appid"].ToLong();
            long uid = 0;

            using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                if (!dbc.CheckTable("AosAuthorize")) {
                    return JsonContent(0, $"请先安装AosAuthorize表");
                }

                if (!dbc.CheckTable("AosUsers")) {
                    return JsonContent(0, $"请先安装AosUsers表");
                }

                if (!dbc.CheckTable("AosApps")) {
                    return JsonContent(0, $"请先安装AosApps表");
                }

                if (!dbc.CheckTable("AosUserApps")) {
                    return JsonContent(0, $"请先安装AosUserApps表");
                }

                if (!dbc.CheckTable("AosUserAuthorize")) {
                    return JsonContent(0, $"请先安装AosUserAuthorize表");
                }

                var row = dbc.GetGdmlOne($"@{{$[AosAuthorize]&[DBSign=='Manage']}}");
                if (row.IsEmpty) {
                    return JsonContent(0, "请先添加管理授权");
                }

                authId = row["ID"].ToLong();

                var rowUser = dbc.GetGdmlOne($"@{{$[AosUsers]&[Name=='root']}}");
                if (rowUser.IsEmpty) {
                    return JsonContent(0, "请先添加管理用户");
                }

                uid = rowUser["ID"].ToLong();

                //添加根用户关联APP
                var rowApp = dbc.GetGdmlOne($"@{{$[AosApps]&[ID=='{appId}']}}");
                if (rowApp.IsEmpty) {
                    return JsonContent(0, "应用不存在，请先添加应用");
                }

                if (dbc.GetGdmlOne($"@{{$[AosUserApps]&[UserID=='{uid}'&&AuthID=='{authId}'&&AppID=='{appId}']}}").IsEmpty)
                    dbc.ExecGdml($"+{{$[AosUserApps].[UserID='{uid}'].[AuthID='{authId}'].[AppID='{appId}'].[Limit='']}}");

            }

            return JsonContent(1);
        }

    }
}