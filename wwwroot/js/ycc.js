/// <reference path="../lib/vue/vue.js" />

//import { setTimeout } from "timers";

var ycc = {};

//核心信息
ycc.info = {
    name: "Yunyitong Core Controller",
    title: "云谊通核心控制器",
    description: "云谊通核心控制器(Yunyitong Core Controller，简称Ycc)是云谊通云应用协作平台的核心组件，是一套基于.Net Core技术研发、包含H5、Web API、Websocket通讯为基础的综合性数据管理系统，为平台提供最基础的用户授权、数据认证和通讯保障。",
    builds: [
        {
            version: "5.0.1901", build: 1,
            notes: [
                "核心技术升级为基于.Net Core 2.2框架，独立进程运行，响应效率更高",
                "支持跨平台部署，可支持将平台部署至Windows、Linux或MacOS",
                "核心通讯升级为WebSocket，网络延迟更低，平台响应速度更快"
            ]
        },
        {
            version: "5.1.1904", build: 2,
            notes: [
                "支持以域名为识别核心的云桌面部署，实现一个控制器针对多个域名采用不同的云桌面",
                "支持云应用仓库概念，大大简化和加快应用添加及部署步骤",
                "更加直观的安装向导，确保安装过程简单明了",
                "兼容应用排序功能，获取应用列表的接口将以排序结果返回"
            ]
        },
        {
            version: "5.2.1905", build: 3,
            notes: [
                "增加缩放限制，兼容手机端操作",
                "增加已Get方式获取用户认证API接口，同时增加API数据验证"
            ]
        },
        {
            version: "5.2.1905", build: 4,
            notes: [
                "增加操作队列，更好的兼容复杂任务",
                "增加框架间消息传输支持,兼容dpz套件的数据传输"
            ]
        },
        {
            version: "5.3.1906", build: 5,
            notes: [
                "修改了一些问题"
            ]
        }
    ],
    getLatestVersion: function () {
        var idx = this.builds.length - 1;
        return this.builds[idx].version + "." + this.builds[idx].build;
    }
};

//ycc.version = "1.2.1904.6";

ycc.socket = null;
ycc.socketAutoMessage = true;
ycc.debug = false;

//交互相关
ycc.session = {
    storage: {
        id: "ycc_session_id",
        key: "ycc_session_key"
    },
    id: "",
    key: ""
};

ycc.eventHandlers = [];
ycc.eventHandlersExecute = function (tp, obj) {
    var idx = ycc.eventHandlers.length;
    for (var i = 0; i < idx; i++) {
        if (!dpz.isNull(ycc.eventHandlers[i])) {
            var handler = ycc.eventHandlers[i];
            if (handler.type === tp || handler.type === "*") {
                if (dpz.isFunction(handler.handler)) handler.handler(obj);
                if (!handler.keep) ycc.eventHandlers[i] = null;
            }
        }
    }
};

ycc.readyHandlers = [];
ycc.readyHandlersExecute = function () {
    var idx = ycc.readyHandlers.length;
    for (var i = 0; i < idx; i++) {
        if (dpz.isFunction(ycc.readyHandlers[i])) ycc.readyHandlers[i]();
    }
    ycc.readyHandlers = [];
};

/**
 * 一次性绑定事件
 * @param {any} tp 事件类型
 * @param {any} fn 回调函数
 */
ycc.bindOnce = function (tp, fn) {
    var idx = ycc.eventHandlers.length;
    var obj = {};
    obj.type = tp;
    obj.handler = fn;
    obj.keep = false;
    ycc.eventHandlers[idx] = obj;
};

/**
 * 绑定事件
 * @param {any} tp 事件类型
 * @param {any} fn 回调函数
 */
ycc.bind = function (tp, fn) {
    var idx = ycc.eventHandlers.length;
    var obj = {};
    obj.type = tp;
    obj.handler = fn;
    obj.keep = true;
    ycc.eventHandlers[idx] = obj;
};

ycc.send = function (content) {
    if (dpz.isNull(ycc.socket)) return;

    if (ycc.debug) console.log("WSSend\\>" + content);
    ycc.socket.send(content);
};

ycc.sendJttp = function (tp, data, fn) {
    ycc.bindOnce(tp, fn);
    var obj = {
        Header: {
            Type: tp,
            SessionID: ycc.session.id
        }
    };
    if (!dpz.isNull(data)) obj.Data = data;
    ycc.send(dpz.data.json.getString(obj));
};

ycc.desktopConfig = {
    Description: "",
    Host: "",
    ID: "",
    Name: "",
    Path: "",
    ScriptEntrance: "",
    Text: "",
    UrlEntrance: ""
};

ycc.entityConfig = {
    ID: "",
    Name: "",
    Code: "",
    SecurityKey: "",
    Lv: "",
    DBSign: "",
    InterfaceUrl: "",
    FileSite: "",
    AppSite: "",
    HomeUrl: "",
    SettingUrl: "",
    LogoUrl: "",
    Status: "",
    desktopID: "",
    CreateUserID: "",
    UrlEntrance: "",
    ScriptEntrance: ""
};

/**
 * 核心控制器就绪事件绑定入口
 * @param {any} fn 绑定函数
 */
ycc.ready = function (fn) {
    var idx = ycc.readyHandlers.length;
    ycc.readyHandlers[idx] = fn;
};

ycc.request = {
    getQueryString: function (item) {
        var svalue = location.search.match(new RegExp("[\?\&]" + item + "=([^\&]*)(\&?)", "i"));
        return svalue ? svalue[1] : svalue;
    }
};

/**
 * 循环队列类
 * */
var YccQueue = {
    create: function () {
        var cls = {};
        var queues = [{ fn: null, interval: 0, tick: 0 }];

        /**
         * 清空队列
         * */
        cls.clear = function () {
            queues = [];
        };

        /**
         * 添加一个队列
         * @param {Function} fn 执行函数
         * @param {Number} m 循环执行间隔时间(分钟)
         * @returns {Number} 队列识别号
         * */
        cls.add = function (fn, m) {
            if (!dpz.isFunction(fn)) throw "缺少执行函数或参数不为执行函数";
            var time = parseInt(m);
            if (isNaN(time)) throw "缺少队列识别号或参数类型不正确";
            if (time <= 0) throw "循环执行间隔时间必须大于0";

            var idx = queues.length;
            queues[idx] = {
                fn: fn,
                interval: time,
                tick: 0
            };
            return idx;
        };

        /**
         * 删除一个队列
         * @param {any} qid 队列识别号
         */
        cls.del = function (qid) {
            var idx = parseInt(qid);
            if (isNaN(idx)) throw "缺少队列识别号或参数类型不正确";
            queues[idx] = null;
        };

        var lastTime = new Date();
        var thread = function () {

            var time = new Date();
            var ts = time - lastTime;
            //console.log("The queue thread timespan is " + ts);

            //以分钟为单位处理
            if (ts >= 60000) {
                console.log("The queue thread is tick on");
                console.log(queues);

                //重新进行急事
                lastTime = new Date();

                //遍历所有队列并执行满足条件的队列
                for (var i = 0; i < queues.length; i++) {
                    if (!dpz.isNull(queues[i])) {
                        queues[i].tick++;
                        var queue = queues[i];
                        if (queue.tick >= queue.interval) {
                            queues[i].tick = 0;
                            queue.fn();
                        }
                    }
                }
            }

            //以秒为单位计时
            setTimeout(thread, 1000);
        };

        //初始化并执行处理线程
        cls.clear();
        thread();

        return cls;
    }
};

/**
 * 循环队列
 * */
ycc.queue = YccQueue.create();

dpz.ready(function () {

    //调试开关
    if (ycc.request.getQueryString("debug") === "1") {
        ycc.debug = true;


        Vue.config.errorHandler = function (err, vm, info) {
            // handle error
            // `info` 是 Vue 特定的错误信息，比如错误所在的生命周期钩子
            // 只在 2.2.0+ 可用
            console.error(err);
            console.error(vm);
            console.error(info);
        };
    }

    //添加消息监听
    dpz.message.listen(function (e) {
        if (ycc.debug) console.log(e);
    });

    //进行交互标识初始化
    var sid = localStorage.getItem(ycc.session.storage.id);
    if (dpz.isNull(sid)) sid = "";
    ycc.session.id = sid;

    var skey = localStorage.getItem(ycc.session.storage.key);
    if (dpz.isNull(skey)) skey = "";
    ycc.session.key = skey;

    //当桌面加载完毕时加载桌面
    ycc.ready(function () {

        //加载桌面
        var host = window.location.host;
        ycc.sendJttp("Desktop.Get", { Host: host }, function (obj) {
            //console.log(obj);
            ycc.desktopConfig = obj.Data.Row;
            //重新设置网页标题
            document.title = ycc.desktopConfig.Text;
            var urlEntrance = ycc.desktopConfig.UrlEntrance;
            var scriptEntrance = ycc.desktopConfig.ScriptEntrance;
            dpz.loader.loadJs("Aos_Js_Desktop", urlEntrance + scriptEntrance);
        });

    });

    //站点加载配置
    site.loadConfig(function () {

        var ws = new WebSocket(site.config.websocket.url);

        ycc.socket = ws;

        //连接成功时，触发事件
        ws.onopen = function () {
            //请求参数
            //var param = { "id": 1, "command": "account_info", "account": "r9cZA1mLK5R5Am25ArfXFmqgNwjZgnfk59" };
            // 使用 send() 方法发送数据
            //ws.send(JSON.stringify(param));
            //alert("数据发送中...");
            //console.log("onopen");

            //调试打印
            console.log("Yunyitong Core Controller is ready!");
            ycc.readyHandlersExecute();
        };

        //接收到服务端响应的数据时，触发事件
        ws.onmessage = function (evt) {
            var data = evt.data;
            if (ycc.debug) console.log("WSMessage\\>" + data);
            var obj = dpz.data.json.parse(data);
            if (!dpz.isNull(obj.Header)) {
                if (ycc.socketAutoMessage) {
                    var status = parseInt(obj.Header.Status);
                    var msg = "";
                    if (!dpz.isNull(obj.Message)) msg += obj.Message;
                    if (status >= 0) {
                        if (msg !== "") alert(msg);
                        //执行绑定函数
                        ycc.eventHandlersExecute(obj.Header.Type, obj);
                    } else {
                        var err = parseInt(obj.Header.Error);
                        if (msg !== "") alert("交互发生异常:\n# 交互类型: " + obj.Header.Type + "\n# 错误码: 0x" + err.toString(16) + "\n# 提示信息: " + msg);
                    }
                } else {
                    //执行绑定函数
                    ycc.eventHandlersExecute(obj.Header.Type, obj);
                }
            } else {
                console.warn("接收到不支持的数据=>" + data);
            }
            //alert("收到数据..." + data);
            //console.log("onmessage=>" + data);
        };

        ws.onerror = function () {
            console.log("onerror");
        };

        // 断开 web socket 连接成功触发事件
        ws.onclose = function () {
            console.log("onclose");
            alert("与服务器连接已断开,请刷新页面重试!");
        };

    });

    //注册控制台命令
    dpz.commands.set("ycc-send", {
        title: "向服务器发送消息",
        params: ["content"]
    }, ycc.send);

    dpz.commands.set("ycc-send-object", {
        title: "向服务器发送对象封装的消息",
        params: ["object"]
    }, function (obj) {
        ycc.send(dpz.data.json.getString(obj));
    });

    //绑定控制台交互
    ycc.bind("Console", function (obj) {
        //console.log("Console\\>" + obj.Data.Command);
        dpz.exec(obj.Data.Command);
    });

});