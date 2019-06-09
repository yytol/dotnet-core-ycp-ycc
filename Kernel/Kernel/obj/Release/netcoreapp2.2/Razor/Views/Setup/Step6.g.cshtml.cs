#pragma checksum "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "66e9a657f0755c91f3f180094b6315be4698cde2"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Setup_Step6), @"mvc.1.0.view", @"/Views/Setup/Step6.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Setup/Step6.cshtml", typeof(AspNetCore.Views_Setup_Step6))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"66e9a657f0755c91f3f180094b6315be4698cde2", @"/Views/Setup/Step6.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e08d644b3aa83c36fb0d721989ed592a395a647", @"/Views/_ViewImports.cshtml")]
    public class Views_Setup_Step6 : global::Microsoft.AspNetCore.Mvc.Razor.RazorPage<dynamic>
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 1 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
  
    //ViewData["Title"] = "Step2";
    Layout = "~/Views/Shared/Setup.cshtml";

    //string xmlString = dpz.Net.Http.GetUTF8($"{site.Config.Orm.XmlUrl}/Setting.xml");
    bool hasAos = false;

    using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Aos)) {
        hasAos = !dbc.GetGdmlOne($"@{{$[AosUsers]&[Name=='root']}}").IsEmpty;
    }


#line default
#line hidden
            BeginContext(386, 107, true);
            WriteLiteral("\r\n<div class=\"box\">\r\n    <div class=\"title\">第6/7步&nbsp;初始化超级管理员</div>\r\n    <div class=\"line\">root用户状态&nbsp;");
            EndContext();
            BeginContext(494, 98, false);
#line 16 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
                               Write(Html.Raw(hasAos ? "<span style='color:#090;'>正常</span>" : "<span style='color:#900;'>未初始化</span>"));

#line default
#line hidden
            EndContext();
            BeginContext(592, 35, true);
            WriteLiteral("</div>\r\n    <div class=\"buttons\">\r\n");
            EndContext();
#line 18 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
         if (!hasAos) {

#line default
#line hidden
            BeginContext(652, 93, true);
            WriteLiteral("            <a id=\"lnkSetup\" href=\"javascript:;\" onclick=\"self.createRoot();\">初始化root用户</a>\r\n");
            EndContext();
#line 20 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
        }

#line default
#line hidden
            BeginContext(756, 40, true);
            WriteLiteral("        <a href=\"/Setup/Step5\">上一步</a>\r\n");
            EndContext();
#line 22 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
         if (hasAos) {

#line default
#line hidden
            BeginContext(820, 44, true);
            WriteLiteral("            <a href=\"/Setup/Step7\">下一步</a>\r\n");
            EndContext();
#line 24 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Step6.cshtml"
        }

#line default
#line hidden
            BeginContext(875, 22, true);
            WriteLiteral("    </div>\r\n</div>\r\n\r\n");
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
