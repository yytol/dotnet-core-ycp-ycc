#pragma checksum "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "22cb74453f040cc44911963e447e32b92e24dd27"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Setup_Step2), @"mvc.1.0.view", @"/Views/Setup/Step2.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Setup/Step2.cshtml", typeof(AspNetCore.Views_Setup_Step2))]
namespace AspNetCore
{
    #line hidden
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Rendering;
    using Microsoft.AspNetCore.Mvc.ViewFeatures;
#line 1 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\_ViewImports.cshtml"
using Kernel;

#line default
#line hidden
#line 2 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\_ViewImports.cshtml"
using Kernel.Models;

#line default
#line hidden
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"22cb74453f040cc44911963e447e32b92e24dd27", @"/Views/Setup/Step2.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e08d644b3aa83c36fb0d721989ed592a395a647", @"/Views/_ViewImports.cshtml")]
    public class Views_Setup_Step2 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
  
    //ViewData["Title"] = "Step2";
    Layout = "~/Views/Shared/Setup.cshtml";

    string xmlString = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Setting.xml");
    bool hasObjects = false;

    using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
        hasObjects = dbc.CheckTable("AosObjects");
    }


#line default
#line hidden
            BeginContext(361, 112, true);
            WriteLiteral("\r\n<div class=\"box\">\r\n    <div class=\"title\">第2/7步&nbsp;安装平台数据表</div>\r\n    <div class=\"line\">配置文件访问地址&nbsp;<span>");
            EndContext();
            BeginContext(475, 22, false);
#line 16 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
                                      Write(site.Config.Orm.XmlUrl);

#line default
#line hidden
            EndContext();
            BeginContext(498, 136, true);
            WriteLiteral("/Setting.xml</span></div>\r\n    <div class=\"line\" style=\"font-weight:bold;font-size:16px;\">平台数据表状态：</div>\r\n    <div class=\"scroll-box\">\r\n");
            EndContext();
#line 19 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
         using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlString)) {
                var xmlDB = xml["database"];
                var xmlAos = xmlDB.GetNodeByAttrValue("name", "Aos", false);
                foreach (var xmlTable in xmlAos.Nodes) {
                    if (xmlTable.Name == "table") {
                        string tabName = xmlTable.Attr["name"];
                        string tabVer = xmlTable.Attr["version"];
                        using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
                            bool tabExist = dbc.CheckTable(tabName);
                            //var row = dbc.GetOne($"Select * From sysObjects Where Name ='{tabName}' And Type In ('S','U')");
                            var rowVer = "无";
                            var opearte = "<i>无需更新</i>";
                            if (tabExist && hasObjects) {
                                var rowObject = dbc.GetGdmlOne($"@{{$[AosObjects]&[Name=='{tabName}']}}");
                                if (!rowObject.IsEmpty) {
                                    rowVer = rowObject["Version"];
                                }
                            }
                            if (rowVer == "无") {
                                opearte = $"<a href='javascript:;' onclick=\"self.createAosTable('{tabName}');\">安装</a>";
                            } else if (tabVer != rowVer) {
                                opearte = $"<a href='javascript:;' onclick=\"self.updateAosTable('{tabName}');\">更新</a>";
                            }

#line default
#line hidden
            BeginContext(2229, 48, true);
            WriteLiteral("        <div class=\"line\">\r\n            <div><s>");
            EndContext();
            BeginContext(2278, 7, false);
#line 43 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
               Write(tabName);

#line default
#line hidden
            EndContext();
            BeginContext(2285, 23, true);
            WriteLiteral("</s>&nbsp;<u>最新版本&nbsp;");
            EndContext();
            BeginContext(2309, 6, false);
#line 43 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
                                              Write(tabVer);

#line default
#line hidden
            EndContext();
            BeginContext(2315, 28, true);
            WriteLiteral("</u></div>\r\n            <div");
            EndContext();
            BeginWriteAttribute("id", " id=\"", 2343, "\"", 2361, 2);
            WriteAttributeValue("", 2348, "div", 2348, 3, true);
#line 44 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
WriteAttributeValue("", 2351, tabName, 2351, 10, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(2362, 60, true);
            WriteLiteral(" style=\"text-indent:14px;padding-top:5px;\">已安装版本&nbsp;<span>");
            EndContext();
            BeginContext(2423, 6, false);
#line 44 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
                                                                                          Write(rowVer);

#line default
#line hidden
            EndContext();
            BeginContext(2429, 13, true);
            WriteLiteral("</span>&nbsp;");
            EndContext();
            BeginContext(2443, 17, false);
#line 44 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
                                                                                                              Write(Html.Raw(opearte));

#line default
#line hidden
            EndContext();
            BeginContext(2460, 24, true);
            WriteLiteral("</div>\r\n        </div>\r\n");
            EndContext();
#line 46 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step2.cshtml"
                        }
                    }
                }

            }

#line default
#line hidden
            BeginContext(2570, 135, true);
            WriteLiteral("    </div>\r\n    <div class=\"buttons\">\r\n        <a href=\"/Setup\">上一步</a>\r\n        <a href=\"/Setup/Step3\">下一步</a>\r\n    </div>\r\n</div>\r\n\r\n");
            EndContext();
        }
        #pragma warning restore 1998
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.ViewFeatures.IModelExpressionProvider ModelExpressionProvider { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IUrlHelper Url { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.IViewComponentHelper Component { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IJsonHelper Json { get; private set; }
        [global::Microsoft.AspNetCore.Mvc.Razor.Internal.RazorInjectAttribute]
        public global::Microsoft.AspNetCore.Mvc.Rendering.IHtmlHelper<dynamic> Html { get; private set; }
    }
}
#pragma warning restore 1591
