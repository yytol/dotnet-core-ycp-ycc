#pragma checksum "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "a987cdf659ddd681b14673b18f89f0e412c777fc"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Setup_Step4), @"mvc.1.0.view", @"/Views/Setup/Step4.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Setup/Step4.cshtml", typeof(AspNetCore.Views_Setup_Step4))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"a987cdf659ddd681b14673b18f89f0e412c777fc", @"/Views/Setup/Step4.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e08d644b3aa83c36fb0d721989ed592a395a647", @"/Views/_ViewImports.cshtml")]
    public class Views_Setup_Step4 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
  
    //ViewData["Title"] = "Step2";
    Layout = "~/Views/Shared/Setup.cshtml";

    string xmlString = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Setting.xml");
    bool hasObjects = false;

    using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Manage)) {
        hasObjects = dbc.CheckTable("SystemObjects");
    }

    string[] xmlNeeds = new string[] { "Base", "Kernel", "Table", "User", "Session" };


#line default
#line hidden
            BeginContext(457, 179, true);
            WriteLiteral("\r\n<div class=\"box\">\r\n    <div class=\"title\">第4/7步&nbsp;安装管理数据表</div>\r\n    <div class=\"line\" style=\"font-weight:bold;font-size:16px;\">管理数据表状态：</div>\r\n    <div class=\"scroll-box\">\r\n");
            EndContext();
#line 20 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
         using (dpz.Data.Xml xml = new dpz.Data.Xml(xmlString)) {
            var xmlDB = xml["database"];
            for (int i = 0; i < xmlNeeds.Length; i++) {
                var xmlAos = xmlDB.GetNodeByAttrValue("name", xmlNeeds[i], false);
                foreach (var xmlTable in xmlAos.Nodes) {
                    if (xmlTable.Name == "table") {
                        string tabName = xmlTable.Attr["name"];
                        string tabVer = xmlTable.Attr["version"];
                        using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Manage)) {
                            //var row = dbc.GetOne($"Select * From sysObjects Where Name ='{tabName}' And Type In ('S','U')");
                            var rowVer = "无";
                            var opearte = "<i>无需更新</i>";
                            if (dbc.CheckTable(tabName) && hasObjects) {
                                var rowObject = dbc.GetGdmlOne($"@{{$[SystemObjects]&[Name=='{tabName}'&&Type=='Table']}}");
                                if (!rowObject.IsEmpty) {
                                    rowVer = rowObject["Version"];
                                }
                            }
                            if (rowVer == "无") {
                                opearte = $"<a href='javascript:;' onclick=\"self.createManageTable('{xmlNeeds[i]}','{tabName}');\">安装</a>";
                            } else if (tabVer != rowVer) {
                                opearte = $"<a href='javascript:;' onclick=\"self.updateManageTable('{xmlNeeds[i]}','{tabName}');\">更新</a>";
                            }

#line default
#line hidden
            BeginContext(2294, 88, true);
            WriteLiteral("                            <div class=\"line\">\r\n                                <div><s>");
            EndContext();
            BeginContext(2383, 7, false);
#line 44 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
                                   Write(tabName);

#line default
#line hidden
            EndContext();
            BeginContext(2390, 23, true);
            WriteLiteral("</s>&nbsp;<u>最新版本&nbsp;");
            EndContext();
            BeginContext(2414, 6, false);
#line 44 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
                                                                  Write(tabVer);

#line default
#line hidden
            EndContext();
            BeginContext(2420, 48, true);
            WriteLiteral("</u></div>\r\n                                <div");
            EndContext();
            BeginWriteAttribute("id", " id=\"", 2468, "\"", 2486, 2);
            WriteAttributeValue("", 2473, "div", 2473, 3, true);
#line 45 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
WriteAttributeValue("", 2476, tabName, 2476, 10, false);

#line default
#line hidden
            EndWriteAttribute();
            BeginContext(2487, 60, true);
            WriteLiteral(" style=\"text-indent:14px;padding-top:5px;\">已安装版本&nbsp;<span>");
            EndContext();
            BeginContext(2548, 6, false);
#line 45 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
                                                                                                              Write(rowVer);

#line default
#line hidden
            EndContext();
            BeginContext(2554, 13, true);
            WriteLiteral("</span>&nbsp;");
            EndContext();
            BeginContext(2568, 17, false);
#line 45 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
                                                                                                                                  Write(Html.Raw(opearte));

#line default
#line hidden
            EndContext();
            BeginContext(2585, 44, true);
            WriteLiteral("</div>\r\n                            </div>\r\n");
            EndContext();
#line 47 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step4.cshtml"
                        }
                    }
                }
            }
        }

#line default
#line hidden
            BeginContext(2724, 141, true);
            WriteLiteral("    </div>\r\n    <div class=\"buttons\">\r\n        <a href=\"/Setup/Step3\">上一步</a>\r\n        <a href=\"/Setup/Step5\">下一步</a>\r\n    </div>\r\n</div>\r\n\r\n");
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
