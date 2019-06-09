#pragma checksum "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "2fefa99c0df7540d16088a7de5ab8e63469d1efe"
// <auto-generated/>
#pragma warning disable 1591
[assembly: global::Microsoft.AspNetCore.Razor.Hosting.RazorCompiledItemAttribute(typeof(AspNetCore.Views_Setup_Index), @"mvc.1.0.view", @"/Views/Setup/Index.cshtml")]
[assembly:global::Microsoft.AspNetCore.Mvc.Razor.Compilation.RazorViewAttribute(@"/Views/Setup/Index.cshtml", typeof(AspNetCore.Views_Setup_Index))]
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
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"2fefa99c0df7540d16088a7de5ab8e63469d1efe", @"/Views/Setup/Index.cshtml")]
    [global::Microsoft.AspNetCore.Razor.Hosting.RazorSourceChecksumAttribute(@"SHA1", @"3e08d644b3aa83c36fb0d721989ed592a395a647", @"/Views/_ViewImports.cshtml")]
    public class Views_Setup_Index : site.RazorPage
    {
        #pragma warning disable 1998
        public async override global::System.Threading.Tasks.Task ExecuteAsync()
        {
#line 2 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml"
  
    //ViewData["Title"] = "View";
    Layout = "~/Views/Shared/Setup.cshtml";

    bool hasAos = false;

    using (dpz.Gdbc.Connection dbc = new dpz.Gdbc.Connection(site.Config.Database.Defend)) {
        //var row = dbc.GetGdmlOne("select * from sys.databases where [name] = 'Aos'");
        hasAos = dbc.CheckDatabase("Aos");
    }

#line default
#line hidden
            BeginContext(377, 107, true);
            WriteLiteral("\r\n<div class=\"box\">\r\n    <div class=\"title\">第1/7步&nbsp;安装平台数据库</div>\r\n    <div class=\"line\">平台数据库安装状态&nbsp;");
            EndContext();
            BeginContext(485, 97, false);
#line 16 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml"
                                Write(Html.Raw(hasAos ? "<span style='color:#090;'>正常</span>" : "<span style='color:#900;'>未安装</span>"));

#line default
#line hidden
            EndContext();
            BeginContext(582, 35, true);
            WriteLiteral("</div>\r\n    <div class=\"buttons\">\r\n");
            EndContext();
#line 18 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml"
         if (!hasAos) {

#line default
#line hidden
            BeginContext(642, 91, true);
            WriteLiteral("            <a id=\"lnkSetup\" href=\"javascript:;\" onclick=\"self.createAos();\">安装AOS数据库</a>\r\n");
            EndContext();
#line 20 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml"
        } else {

#line default
#line hidden
            BeginContext(751, 44, true);
            WriteLiteral("            <a href=\"/Setup/Step2\">下一步</a>\r\n");
            EndContext();
#line 22 "X:\Projects\Private\P18013_Ydm5\trunk\Kernel\Kernel\Views\Setup\Index.cshtml"
        }

#line default
#line hidden
            BeginContext(806, 22, true);
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
