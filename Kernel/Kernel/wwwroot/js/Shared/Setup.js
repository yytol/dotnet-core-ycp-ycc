/// <reference path="../../lib/jquery/dist/jquery.min.js" />
/// <reference path="../../lib/dpz/dpz_1.2.1902.3.js" />

//import { SSL_OP_SINGLE_DH_USE } from "constants";

$(function () {

    var self = window.self = new Object();

    //创建平台数据库
    self.createAos = function () {
        $.get("/Setup/CreateAos", function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("创建成功!");
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    }

    //创建平台数据表
    self.createAosTable = function (tab) {
        $("#div" + tab).html("安装中...");
        $.post("/Setup/CreateAosTable", { table: tab }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                //alert("安装成功!");
                //location.reload(true);
                $("#div" + tab).html("已安装版本&nbsp;<span>" + res.Version + "</span>&nbsp;<i>无需更新</i>");
            } else {
                alert(res.Message);
            }
        });
    }

    //更新平台数据表
    self.updateAosTable = function (tab) {
        $("#div" + tab).html("更新中...");
        $.post("/Setup/CreateAosTable", { table: tab }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("更新成功!");
                //location.reload(true);
                $("#div" + tab).html("已安装版本&nbsp;<span>" + res.Version + "</span>&nbsp;<i>无需更新</i>");
            } else {
                alert(res.Message);
            }
        });
    }

    //创建管理数据库
    self.createManage = function () {
        $.get("/Setup/CreateManage", function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                if (res.Code) {
                    var copyData = "AuthID:" + res.Code + "\nSecurityKey:" + res.SecurityKey;
                    alert("创建成功!\n\n以下为管理专用授权的授权信息，请务必妥善保存（此信息只出现一次）\n\n" + copyData);
                } else {
                    alert("创建成功!");
                }
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    }

    //创建平台数据表
    self.createManageTable = function (plm, tab) {
        $("#div" + tab).html("安装中...");
        $.post("/Setup/CreateManageTable", { platform: plm, table: tab }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                //alert("安装成功!");
                //location.reload(true);
                $("#div" + tab).html("已安装版本&nbsp;<span>" + res.Version + "</span>&nbsp;<i>无需更新</i>");
            } else {
                alert(res.Message);
            }
        });
    }

    //更新平台数据表
    self.updateManageTable = function (plm, tab) {
        $("#div" + tab).html("更新中...");
        $.post("/Setup/CreateManageTable", { platform: plm, table: tab }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                //alert("更新成功!");
                //location.reload(true);
                $("#div" + tab).html("已安装版本&nbsp;<span>" + res.Version + "</span>&nbsp;<i>无需更新</i>");
            } else {
                alert(res.Message);
            }
        });
    }

    //创建应用
    self.createApp = function () {
        $.get("/Setup/CreateApp", function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("创建成功!");
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    };

    //创建或更新管理应用
    self.createManageApp = function (sign) {
        $.post("/Setup/CreateManageApp", { sign: sign }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("更新成功!");
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    };

    //创建应用
    self.createRoot = function () {
        $.get("/Setup/CreateRoot", function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("初始化成功!\n\nroot初始密码：123456");
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    }

    //创建应用
    self.createRootUserApp = function (appid) {
        $.post("/Setup/CreateRootUserApp", { appid: appid }, function (e) {
            var res = dpz.data.json.parse(e);
            var status = parseInt(res.Status);
            if (status > 0) {
                alert("关联成功");
                location.reload(true);
            } else {
                alert(res.Message);
            }
        });
    }

});