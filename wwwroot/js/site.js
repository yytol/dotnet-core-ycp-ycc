// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
var site = {};

site.config = {
    websocket: {
        url: ""
    }
};

site.loadConfig = function (fn) {
    $.get("/Api/Ycc/GetConfig", function (data) {
        //alert(data);
        var jttp = dpz.data.json.parse(data);
        site.config = jttp.Data;
        if (dpz.isFunction(fn)) fn();
    });
};
