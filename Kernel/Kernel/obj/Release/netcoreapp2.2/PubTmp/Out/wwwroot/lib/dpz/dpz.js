/**
 * 大胖子命令管理器
 * */
function DpzCommandsManager() {
    this.items = [];
}

/**
 * 设置命令
 * @param {string} name 命令名称
 * @param {string} description 命令描述
 * @param {void} fn 命令绑定函数
 */
DpzCommandsManager.prototype.set = function (name, description, fn) {
    var list = this.items;
    var idxs = list.length;

    var des = {};
    if (typeof description === "string") {
        des.title = "" + description;
        if (des.title === "") des.title = "无";
    } else if (typeof description === "object") {
        des = description;
    } else if (typeof description === "function") {
        des.title = "无";
        if (dpz.isNull(fn)) {
            fn = description;
        } else {
            throw "参数类型错误";
        }
    } else {
        throw "参数类型错误";
    }

    //by函数接受一个成员名字符串做为参数
    //并返回一个可以用来对包含该成员的对象数组进行排序的比较函数
    var by = function (name) {
        return function (o, p) {
            var a, b;
            if (typeof o === "object" && typeof p === "object" && o && p) {
                a = o[name];
                b = p[name];
                if (a === b) {
                    return 0;
                }
                if (typeof a === typeof b) {
                    return a < b ? -1 : 1;
                }
                return typeof a < typeof b ? -1 : 1;
            }
            else {
                throw "error";
            }
        };
    };

    if (typeof fn === "undefined") {
        if (typeof description === "function") {
            fn = description;
            description = undefined;
        } else {
            throw "缺少函数定义";
        }
    } else {
        if (typeof fn !== "function") {
            throw "缺少函数定义";
        }
    }

    for (var i = 0; i < idxs; i++) {
        if (list[i].name === name) {
            list[i].description = des;
            list[i].handler = fn;
            return;
        }
    }
    list[idxs] = {
        name: name,
        description: des,
        handler: fn
    };
    list.sort(by("Name"));
};

/**
 * 大胖子动态加载器
 * */
function DpzDynamicLoader() {
    this.styles = [];
    this.scripts = [];
    this.images = [];
}

/**
 * 添加一个Css文件
 * @param {string} id 唯一标识符
 * @param {string} url 地址
 */
DpzDynamicLoader.prototype.addCss = function (id, url) {
    var idx = this.styles.length;
    this.styles[idx] = { id: id, url: url };
};

/**
 * 添加一个Js文件
 * @param {string} id 唯一标识符
 * @param {string} url 地址
 */
DpzDynamicLoader.prototype.addJs = function (id, url) {
    var idx = this.scripts.length;
    this.scripts[idx] = { id: id, url: url };
};

/**
 * 添加一张图片
 * @param {string} url 地址
 */
DpzDynamicLoader.prototype.addImage = function (url) {
    var idx = this.images.length;
    this.images[idx] = url;
};

/**
 * 加载一个样式文件
 * @param {string} id  唯一标识符
 * @param {string} url 地址
 * @param {void} fn 回调函数
 */
DpzDynamicLoader.prototype.loadCss = function (id, url, fn) {

    if (url.indexOf("?") > 0) {
        url += "&rnd=" + Math.random();
    } else {
        url += "?rnd=" + Math.random();
    }

    var js = document.getElementById(id);
    if (!js) {
        js = document.createElement("link");
        js.id = id;
        js.href = url;
        js.rel = "stylesheet";

        js.onload = js.onreadystatechange = function () {
            if (!this.readyState || this.readyState === 'loaded' || this.readyState === 'complete') {
                //alert('done');
                if (fn) fn();
            }
            js.onload = js.onreadystatechange = null;
        };

        document.head.appendChild(js);
        //while (!X.Configs.Completed[name]) { }
    } else {
        if (fn) fn();
    }
};

/**
 * 加载一个脚本文件
 * @param {string} id  唯一标识符
 * @param {string} url 地址
 * @param {void} fn 回调函数
 */
DpzDynamicLoader.prototype.loadJs = function (id, url, fn) {
    //var id = "X_" + name
    if (url.indexOf("?") > 0) {
        url += "&rnd=" + Math.random();
    } else {
        url += "?rnd=" + Math.random();
    }

    var js = document.getElementById(id);
    if (!js) {
        js = document.createElement("script");
        js.id = id;
        js.src = url;

        js.onload = js.onreadystatechange = function () {
            if (!this.readyState || this.readyState === 'loaded' || this.readyState === 'complete') {
                //alert('done');
                //执行大胖子专用入口绑定函数
                dpz.initializer.handlersExecute();
                if (fn) fn();
            }
            js.onload = js.onreadystatechange = null;
        };

        document.head.appendChild(js);
        //while (!X.Configs.Completed[name]) { }
    } else {
        if (fn) fn();
    }
};

/**
 * 动态加载图片
 * @param {string} url 地址
 * @param {void} fun 回调函数
 * @param {void} errfun 发生错误时的回调函数
 */
DpzDynamicLoader.prototype.loadImage = function (url, fun, errfun) {
    var Img = new Image();

    Img.onerror = function () {
        if (errfun) errfun({ Url: url, Image: Img });
        if (fun) fun({ Url: url, Image: Img });
    };

    Img.onload = function () {
        if (fun) fun({ Url: url, Image: Img });
    };

    Img.src = url;
};

/**
 * 执行加载
 * @param {void} fn 回调函数
 */
DpzDynamicLoader.prototype.load = function (fn) {

    var my = this;
    var idx = 0;

    //加载所有的样式文件
    var loadStyles = function (fnStyles) {
        if (idx >= my.styles.length) {
            if (typeof fnStyles === "function") fnStyles();
            return;
        }

        //加载样式
        my.loadCss(my.styles[idx].id, my.styles[idx].url, function () {
            idx++;
            loadStyles(fnStyles);
        });
    };

    //加载所有的图片文件
    var loadImages = function (fnImages) {
        if (idx >= my.images.length) {
            if (typeof fnImages === "function") fnImages();
            return;
        }

        //加载样式
        my.loadImage(my.images[idx], function () {
            idx++;
            loadImages(fnImages);
        });
    };

    //加载所有的图片文件
    var loadScripts = function (fnScripts) {
        if (idx >= my.scripts.length) {
            if (typeof fnScripts === "function") fnScripts();
            return;
        }

        //加载样式
        my.loadJs(my.scripts[idx].id, my.scripts[idx].url, function () {
            idx++;
            loadScripts(fnScripts);
        });
    };

    loadStyles(function () {
        idx = 0;
        loadImages(function () {
            idx = 0;
            loadScripts(function () {
                my.styles = [];
                my.images = [];
                my.scripts = [];
                if (typeof fn === "function") fn();
            });
        });
    });
};

/**
 * 大胖子表单管理
 * @param {string} ele 表单所属对象
 * */
function DpzForm(ele) {
    this.form = ele;
}

DpzForm.prototype.getValues = function () {
    var i = 0;
    //var ele = document.createElement("div");
    var ele = this.form;
    var res = {};

    //处理所有的Input
    var inputs = ele.getElementsByTagName("input");
    for (i = 0; i < inputs.length; i++) {
        var input = inputs[i];
        if (!dpz.isNull(input.name)) {
            if (input.name !== "") {
                res[input.name] = input.value;
            }
        }
    }

    //处理所有的texteare
    var txts = ele.getElementsByTagName("textarea");
    for (i = 0; i < txts.length; i++) {
        var txt = txts[i];
        if (!dpz.isNull(txt.name)) {
            if (txt.name !== "") {
                res[txt.name] = txt.value;
            }
        }
    }

    //处理所有的texteare
    var selects = ele.getElementsByTagName("select");
    for (i = 0; i < selects.length; i++) {
        var select = selects[i];
        if (!dpz.isNull(select.name)) {
            if (select.name !== "") {
                res[select.name] = select.value;
            }
        }
    }

    return res;
};

/**
 * 大胖子专用UTF8处理类
 * */
var DpzEncodeUtf8 = {

    /**
    * 对象实例化
    * @returns {Object} 实例化后的对象
    * */
    create: function () {
        var res = {};

        /**
         * 将字符串内容转化为字节数组
         * @param {string} str 字符串
         * @returns {Array} 字节数组
         * */
        res.getBytes = function (str) {
            var back = [];
            var byteSize = 0;
            for (var i = 0; i < str.length; i++) {
                var code = str.charCodeAt(i);
                if (0x00 <= code && code <= 0x7f) {
                    byteSize += 1;
                    back.push(code);
                }
                else if (0x80 <= code && code <= 0x7ff) {
                    byteSize += 2;
                    back.push((192 | (31 & (code >> 6))));
                    back.push((128 | (63 & code)));
                }
                else if ((0x800 <= code && code <= 0xd7ff)
                    || (0xe000 <= code && code <= 0xffff)) {
                    byteSize += 3;
                    back.push((224 | (15 & (code >> 12))));
                    back.push((128 | (63 & (code >> 6))));
                    back.push((128 | (63 & code)));
                }
            }
            for (i = 0; i < back.length; i++) {
                back[i] &= 0xff;
            }
            return back;
        };

        /**
        * 将字节数组内容转化为字符串
        * @param {Array} arr 字节数组
        * @returns {string} 字符串
        * */
        res.getString = function (arr) {
            if (typeof arr === 'string') {
                return arr;
            }
            var UTF = '', _arr = this.init(arr);
            for (var i = 0; i < _arr.length; i++) {
                var one = _arr[i].toString(2), v = one.match(/^1+?(?=0)/);
                if (v && one.length == 8) {
                    var bytesLength = v[0].length;
                    var store = _arr[i].toString(2).slice(7 - bytesLength);
                    for (var st = 1; st < bytesLength; st++) {
                        store += _arr[st + i].toString(2).slice(2);
                    }
                    UTF += String.fromCharCode(parseInt(store, 2));
                    i += bytesLength - 1;
                }
                else {
                    UTF += String.fromCharCode(_arr[i]);
                }
            }
            return UTF;
        }

        return res;
    }
};

/**
 * MD5处理类
 * */
var DpzSecurity = {

    /**
    * 对象实例化
    * @returns {Object} 实例化后的对象
    * */
    create: function () {
        var res = {};

        /**
         * 获取字符串的MD5值
         * @param {string} s 字符串信息
         * @returns {string} 字符串MD5值
         * */
        res.getMD5 = function (s) {
            var dykMD5 = new Object();
            dykMD5.hexcase = 1; /* hex output format. 0 - lowercase; 1 - uppercase */
            dykMD5.b64pad = ""; /* base-64 pad character. "=" for strict RFC compliance */
            dykMD5.chrsz = 8; /* bits per input character. 8 - ASCII; 16 - Unicode */
            /*
            * These are the functions you'll usually want to call
            * They take string arguments and return either hex or base-64 encoded strings
            */
            dykMD5.GetHexMD5 = function (s) {
                //var bytes = dykEncode.Utf8.GetBytes(s);
                var utf8 = DpzEncodeUtf8.create();
                var bytes = utf8.getBytes(s);
                return dykMD5.binl2hex(dykMD5.core_md5(dykMD5.arr2binl(bytes), bytes.length * dykMD5.chrsz));
                //return dykMD5.binl2hex(dykMD5.core_md5(dykMD5.str2binl(s), s.length * dykMD5.chrsz));
            };
            dykMD5.GetB64MD5 = function (s) { return dykMD5.binl2b64(dykMD5.core_md5(dykMD5.str2binl(s), s.length * dykMD5.chrsz)); };
            dykMD5.GetHexHMacMD5 = function (key, data) { return dykMD5.binl2hex(dykMD5.core_hmac_md5(key, data)); };
            dykMD5.GetB64HMacMD5 = function (key, data) { return dykMD5.binl2b64(dykMD5.core_hmac_md5(key, data)); };
            /* Backwards compatibility - same as hex_md5() */
            dykMD5.calcMD5 = function (s) { return dykMD5.binl2hex(dykMD5.core_md5(dykMD5.str2binl(s), s.length * dykMD5.chrsz)); };
            /*
            * Perform a simple self-test to see if the VM is working
            */
            dykMD5.md5_vm_test = function () {
                return hex_md5("abc") == "900150983cd24fb0d6963f7d28e17f72";
            };
            /*
            * Calculate the MD5 of an array of little-endian words, and a bit length
            */
            dykMD5.core_md5 = function (x, len) {
                /* append padding */
                x[len >> 5] |= 0x80 << ((len) % 32);
                x[(((len + 64) >>> 9) << 4) + 14] = len;
                var a = 1732584193;
                var b = -271733879;
                var c = -1732584194;
                var d = 271733878;
                for (var i = 0; i < x.length; i += 16) {
                    var olda = a;
                    var oldb = b;
                    var oldc = c;
                    var oldd = d;
                    a = dykMD5.md5_ff(a, b, c, d, x[i + 0], 7, -680876936);
                    d = dykMD5.md5_ff(d, a, b, c, x[i + 1], 12, -389564586);
                    c = dykMD5.md5_ff(c, d, a, b, x[i + 2], 17, 606105819);
                    b = dykMD5.md5_ff(b, c, d, a, x[i + 3], 22, -1044525330);
                    a = dykMD5.md5_ff(a, b, c, d, x[i + 4], 7, -176418897);
                    d = dykMD5.md5_ff(d, a, b, c, x[i + 5], 12, 1200080426);
                    c = dykMD5.md5_ff(c, d, a, b, x[i + 6], 17, -1473231341);
                    b = dykMD5.md5_ff(b, c, d, a, x[i + 7], 22, -45705983);
                    a = dykMD5.md5_ff(a, b, c, d, x[i + 8], 7, 1770035416);
                    d = dykMD5.md5_ff(d, a, b, c, x[i + 9], 12, -1958414417);
                    c = dykMD5.md5_ff(c, d, a, b, x[i + 10], 17, -42063);
                    b = dykMD5.md5_ff(b, c, d, a, x[i + 11], 22, -1990404162);
                    a = dykMD5.md5_ff(a, b, c, d, x[i + 12], 7, 1804603682);
                    d = dykMD5.md5_ff(d, a, b, c, x[i + 13], 12, -40341101);
                    c = dykMD5.md5_ff(c, d, a, b, x[i + 14], 17, -1502002290);
                    b = dykMD5.md5_ff(b, c, d, a, x[i + 15], 22, 1236535329);
                    a = dykMD5.md5_gg(a, b, c, d, x[i + 1], 5, -165796510);
                    d = dykMD5.md5_gg(d, a, b, c, x[i + 6], 9, -1069501632);
                    c = dykMD5.md5_gg(c, d, a, b, x[i + 11], 14, 643717713);
                    b = dykMD5.md5_gg(b, c, d, a, x[i + 0], 20, -373897302);
                    a = dykMD5.md5_gg(a, b, c, d, x[i + 5], 5, -701558691);
                    d = dykMD5.md5_gg(d, a, b, c, x[i + 10], 9, 38016083);
                    c = dykMD5.md5_gg(c, d, a, b, x[i + 15], 14, -660478335);
                    b = dykMD5.md5_gg(b, c, d, a, x[i + 4], 20, -405537848);
                    a = dykMD5.md5_gg(a, b, c, d, x[i + 9], 5, 568446438);
                    d = dykMD5.md5_gg(d, a, b, c, x[i + 14], 9, -1019803690);
                    c = dykMD5.md5_gg(c, d, a, b, x[i + 3], 14, -187363961);
                    b = dykMD5.md5_gg(b, c, d, a, x[i + 8], 20, 1163531501);
                    a = dykMD5.md5_gg(a, b, c, d, x[i + 13], 5, -1444681467);
                    d = dykMD5.md5_gg(d, a, b, c, x[i + 2], 9, -51403784);
                    c = dykMD5.md5_gg(c, d, a, b, x[i + 7], 14, 1735328473);
                    b = dykMD5.md5_gg(b, c, d, a, x[i + 12], 20, -1926607734);
                    a = dykMD5.md5_hh(a, b, c, d, x[i + 5], 4, -378558);
                    d = dykMD5.md5_hh(d, a, b, c, x[i + 8], 11, -2022574463);
                    c = dykMD5.md5_hh(c, d, a, b, x[i + 11], 16, 1839030562);
                    b = dykMD5.md5_hh(b, c, d, a, x[i + 14], 23, -35309556);
                    a = dykMD5.md5_hh(a, b, c, d, x[i + 1], 4, -1530992060);
                    d = dykMD5.md5_hh(d, a, b, c, x[i + 4], 11, 1272893353);
                    c = dykMD5.md5_hh(c, d, a, b, x[i + 7], 16, -155497632);
                    b = dykMD5.md5_hh(b, c, d, a, x[i + 10], 23, -1094730640);
                    a = dykMD5.md5_hh(a, b, c, d, x[i + 13], 4, 681279174);
                    d = dykMD5.md5_hh(d, a, b, c, x[i + 0], 11, -358537222);
                    c = dykMD5.md5_hh(c, d, a, b, x[i + 3], 16, -722521979);
                    b = dykMD5.md5_hh(b, c, d, a, x[i + 6], 23, 76029189);
                    a = dykMD5.md5_hh(a, b, c, d, x[i + 9], 4, -640364487);
                    d = dykMD5.md5_hh(d, a, b, c, x[i + 12], 11, -421815835);
                    c = dykMD5.md5_hh(c, d, a, b, x[i + 15], 16, 530742520);
                    b = dykMD5.md5_hh(b, c, d, a, x[i + 2], 23, -995338651);
                    a = dykMD5.md5_ii(a, b, c, d, x[i + 0], 6, -198630844);
                    d = dykMD5.md5_ii(d, a, b, c, x[i + 7], 10, 1126891415);
                    c = dykMD5.md5_ii(c, d, a, b, x[i + 14], 15, -1416354905);
                    b = dykMD5.md5_ii(b, c, d, a, x[i + 5], 21, -57434055);
                    a = dykMD5.md5_ii(a, b, c, d, x[i + 12], 6, 1700485571);
                    d = dykMD5.md5_ii(d, a, b, c, x[i + 3], 10, -1894986606);
                    c = dykMD5.md5_ii(c, d, a, b, x[i + 10], 15, -1051523);
                    b = dykMD5.md5_ii(b, c, d, a, x[i + 1], 21, -2054922799);
                    a = dykMD5.md5_ii(a, b, c, d, x[i + 8], 6, 1873313359);
                    d = dykMD5.md5_ii(d, a, b, c, x[i + 15], 10, -30611744);
                    c = dykMD5.md5_ii(c, d, a, b, x[i + 6], 15, -1560198380);
                    b = dykMD5.md5_ii(b, c, d, a, x[i + 13], 21, 1309151649);
                    a = dykMD5.md5_ii(a, b, c, d, x[i + 4], 6, -145523070);
                    d = dykMD5.md5_ii(d, a, b, c, x[i + 11], 10, -1120210379);
                    c = dykMD5.md5_ii(c, d, a, b, x[i + 2], 15, 718787259);
                    b = dykMD5.md5_ii(b, c, d, a, x[i + 9], 21, -343485551);
                    a = dykMD5.safe_add(a, olda);
                    b = dykMD5.safe_add(b, oldb);
                    c = dykMD5.safe_add(c, oldc);
                    d = dykMD5.safe_add(d, oldd);
                }
                return Array(a, b, c, d);
            };
            /*
            * These functions implement the four basic operations the algorithm uses.
            */
            dykMD5.md5_cmn = function (q, a, b, x, s, t) {
                return dykMD5.safe_add(dykMD5.bit_rol(dykMD5.safe_add(dykMD5.safe_add(a, q), dykMD5.safe_add(x, t)), s), b);
            };
            dykMD5.md5_ff = function (a, b, c, d, x, s, t) {
                return dykMD5.md5_cmn((b & c) | ((~b) & d), a, b, x, s, t);
            };
            dykMD5.md5_gg = function (a, b, c, d, x, s, t) {
                return dykMD5.md5_cmn((b & d) | (c & (~d)), a, b, x, s, t);
            };
            dykMD5.md5_hh = function (a, b, c, d, x, s, t) {
                return dykMD5.md5_cmn(b ^ c ^ d, a, b, x, s, t);
            };
            dykMD5.md5_ii = function (a, b, c, d, x, s, t) {
                return dykMD5.md5_cmn(c ^ (b | (~d)), a, b, x, s, t);
            };
            /*
            * Calculate the HMAC-MD5, of a key and some data
            */
            dykMD5.core_hmac_md5 = function (key, data) {
                var bkey = dykMD5.str2binl(key);
                if (bkey.length > 16)
                    bkey = dykMD5.core_md5(bkey, key.length * dykMD5.chrsz);
                var ipad = Array(16), opad = Array(16);
                for (var i = 0; i < 16; i++) {
                    ipad[i] = bkey[i] ^ 0x36363636;
                    opad[i] = bkey[i] ^ 0x5C5C5C5C;
                }
                var hash = dykMD5.core_md5(ipad.concat(dykMD5.str2binl(data)), 512 + data.length * dykMD5.chrsz);
                return dykMD5.core_md5(opad.concat(hash), 512 + 128);
            };
            /*
            * Add integers, wrapping at 2^32. This uses 16-bit operations internally
            * to work around bugs in some JS interpreters.
            */
            dykMD5.safe_add = function (x, y) {
                var lsw = (x & 0xFFFF) + (y & 0xFFFF);
                var msw = (x >> 16) + (y >> 16) + (lsw >> 16);
                return (msw << 16) | (lsw & 0xFFFF);
            };
            /*
            * Bitwise rotate a 32-bit number to the left.
            */
            dykMD5.bit_rol = function (num, cnt) {
                return (num << cnt) | (num >>> (32 - cnt));
            };
            /*
            * Convert a string to an array of little-endian words
            * If chrsz is ASCII, characters >255 have their hi-byte silently ignored.
            */
            dykMD5.str2binl = function (str) {
                //Unicode方式
                //var bin = Array();
                //var mask = (1 << dykMD5.chrsz) - 1;
                //for (var i = 0; i < str.length * dykMD5.chrsz; i += dykMD5.chrsz)
                //    bin[i >> 5] |= (str.charCodeAt(i / dykMD5.chrsz) & mask) << (i % 32);
                //return bin;
                var utf8 = new DykJsDevelopmentKitEncodeUtf8();
                var bytes = utf8.GetBytes(str);
                var bin = Array();
                var mask = (1 << dykMD5.chrsz) - 1;
                for (var i = 0; i < bytes.length * dykMD5.chrsz; i += dykMD5.chrsz)
                    bin[i >> 5] |= (bytes[i / dykMD5.chrsz] & mask) << (i % 32);
                return bin;
                //return dykEncode.Utf8.GetBytes(str);
            };
            dykMD5.arr2binl = function (arr) {
                //Unicode方式
                //var bin = Array();
                //var mask = (1 << dykMD5.chrsz) - 1;
                //for (var i = 0; i < str.length * dykMD5.chrsz; i += dykMD5.chrsz)
                //    bin[i >> 5] |= (str.charCodeAt(i / dykMD5.chrsz) & mask) << (i % 32);
                //return bin;
                //var bytes = dykEncode.Utf8.GetBytes(str);
                var bin = Array();
                var mask = (1 << dykMD5.chrsz) - 1;
                for (var i = 0; i < arr.length * dykMD5.chrsz; i += dykMD5.chrsz)
                    bin[i >> 5] |= (arr[i / dykMD5.chrsz] & mask) << (i % 32);
                return bin;
                //return dykEncode.Utf8.GetBytes(str);
            };
            /*
            * Convert an array of little-endian words to a hex string.
            */
            dykMD5.binl2hex = function (binarray) {
                var hex_tab = dykMD5.hexcase ? "0123456789ABCDEF" : "0123456789abcdef";
                var str = "";
                for (var i = 0; i < binarray.length * 4; i++) {
                    str += hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8 + 4)) & 0xF) +
                        hex_tab.charAt((binarray[i >> 2] >> ((i % 4) * 8)) & 0xF);
                }
                return str;
            };
            /*
            * Convert an array of little-endian words to a base-64 string
            */
            dykMD5.binl2b64 = function (binarray) {
                var tab = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/";
                var str = "";
                for (var i = 0; i < binarray.length * 4; i += 3) {
                    var triplet = (((binarray[i >> 2] >> 8 * (i % 4)) & 0xFF) << 16)
                        | (((binarray[i + 1 >> 2] >> 8 * ((i + 1) % 4)) & 0xFF) << 8)
                        | ((binarray[i + 2 >> 2] >> 8 * ((i + 2) % 4)) & 0xFF);
                    for (var j = 0; j < 4; j++) {
                        if (i * 8 + j * 6 > binarray.length * 32)
                            str += dykMD5.b64pad;
                        else
                            str += tab.charAt((triplet >> 6 * (3 - j)) & 0x3F);
                    }
                }
                return str;
            };

            //return dykMD5;
            return dykMD5.GetHexMD5(s);
        }

        return res;
    }
};

/**
* 大胖子软件工作室专用JS开发套件
* */
var dpz = {};
dpz.isArray = function (obj) { return Object.prototype.toString.call(obj) === '[object Array]'; };
dpz.isFunction = function (obj) { return typeof obj === 'function'; };
dpz.isNull = function (obj) {
    switch (typeof obj) {
        case "undefined":
            return true;
        case "object":
            return obj === null;
        default:
            return false;
    }
};

/**
* 大胖子套件信息
* */
dpz.info = {
    name: "D.P.Z Development Kit for Javascript",
    version: "1.4.1905",
    build: 5,
    owner: "大胖子软件工作室",
    getVersion: function () {
        var that = this;
        return that.version + "." + that.build;
    },
    toString: function () {
        var that = this;
        return that.name + " Ver " + that.getVersion();
    }
};

/**
 * 大胖子安全类专用操作对象
 * */
dpz.security = DpzSecurity.create();

/**
 * 大胖子Dom专用操作对象
 * */
dpz.element = {
    getForm: function (ele) { return new DpzForm(ele); },
    getFormById: function (id) { return new DpzForm(document.getElementById(id)); }
};

/**
 * 大胖子专用数据操作对象
 * */
dpz.data = {

    /**
     * Json操作对象
     * */
    json: {

        /**
         * 从对象获取标准json字符串
         * @param {object} obj 命令绑定函数
         * @returns {string} 标准json字符串
         * */
        getString: function (obj) {
            var res = "";
            var my = this;
            if (typeof obj === "object") {
                //为对象
                if (dpz.isArray(obj)) {
                    //为数组
                    for (p in obj) {
                        if (typeof obj[p] !== "function") {
                            if (res !== "")
                                res += ",";
                            res += my.getString(obj[p]);
                        }
                    }
                    return "[" + res + "]";
                }
                else {
                    //不为数组
                    for (p in obj) {
                        if (typeof obj[p] !== "function") {
                            if (res !== "")
                                res += ",";
                            res += "\"" + p + "\":";
                            res += my.getString(obj[p]);
                        }
                    }
                    return "{" + res + "}";
                }
            }
            else {
                return "\"" + obj + "\"";
            }
        },

        /**
         * 将标准json字符串转化为
         * @param {string} s 命令绑定函数
         * @returns {object} 标准json字符串
         * */
        parse: function (s) {
            try {
                var obj = eval('(' + s + ')');
                return obj;
            }
            catch (e) {
                console.error("转换对象发生异常!");
                console.error(s);
                return {};
            }
        }
    }
};

/**
 * 大胖子控制台
 * */
dpz.console = {

    lastCommandResult: "",
    lastCommand: "",
    lastCommandScript: "",
    lastErrorCommand: "",
    lastErrorCommandScript: "",

    /**
    * 命令管理器
    * */
    commands: new DpzCommandsManager(),

    /**
     * 执行命令
     * @param {string} cmd 命令字符串
     * @returns {any} 命令返回
     */
    exec: function (cmd) {
        //var arr = dcr.getUtf8Bytes(str);
        var res = "";
        var sz = "" + cmd;
        var c160 = String.fromCharCode(160);
        var c32 = String.fromCharCode(32);
        //var isNote = false;
        var doubleString = false;
        var singleString = false;
        var flag = false;
        var tempString = "";
        var command = "";
        var args = new Array();

        //console.log("-> " + sz);

        //遍历字符串
        var chr, idx = 0;
        do {
            chr = sz.charAt(idx);

            if (chr !== "") {
                switch (chr) {
                    case '"':
                        if (flag) {
                            tempString += "\"";
                            flag = false;
                        } else {
                            if (!doubleString) {
                                //当前面还有代码，则不处理该代码
                                //if (tempString != "") {
                                //    return "字符串定义不符合规范";
                                //}
                                tempString += "\"";
                                doubleString = true;
                            } else {
                                //if (command == "") {
                                //    command = tempString;
                                //} else {
                                //    args[args.length] = tempString;
                                //}
                                tempString += "\"";
                                //tempString = "";
                                doubleString = false;
                            }
                        }
                        break;
                    case '\'':
                        if (!doubleString) {
                            if (singleString) {
                                //if (command == "") {
                                //    command = tempString;
                                //} else {
                                //    args[args.length] = tempString;
                                //}
                                tempString += "'";
                                //tempString = "";
                                singleString = false;
                            } else {
                                //if (tempString != "") {
                                //    return "字符串定义不符合规范";
                                //}
                                tempString += "'";
                                singleString = true;
                            }
                        } else {
                            tempString += "'";
                        }
                        break;
                    case '\\':
                        if (flag) {
                            tempString += "\\\\";
                            flag = false;
                        } else {
                            if (doubleString || singleString) {
                                flag = true;
                            }
                        }
                        break;
                    case 'n':
                        if (flag) {
                            tempString += "\\" + chr;
                            flag = false;
                        } else {
                            tempString += chr;
                        }
                        break;
                    case c160:
                    case c32:
                        //空格处理
                        if (!(doubleString || singleString)) {
                            //单词结束
                            if (tempString !== "") {
                                //res += dcrInside.getKeyCode(dcrParse.tempString);
                                if (command === "") {
                                    command = tempString;
                                } else {
                                    args[args.length] = tempString;
                                }
                                tempString = "";
                            }
                            //res += "&nbsp;";
                        } else {
                            tempString += " ";
                        }
                        break;
                    default:
                        if (flag) throw "不支持的转义";
                        tempString += chr;
                        break;
                }
            }
            idx++;

        } while (chr !== "");

        if (tempString !== "") {
            if (command === "") {
                command = tempString;
            } else {
                args[args.length] = tempString;
            }
            tempString = "";
        }

        var idxs = dpz.console.commands.items.length;
        idx = -1;
        var i = 0;
        for (i = 0; i < idxs; i++) {
            if (dpz.console.commands.items[i].name === command) {
                idx = i;
                break;
            }
        }

        var script = "dpz.console.lastCommandResult = dpz.console.commands.items[\"" + idx + "\"].handler(";
        for (i = 0; i < args.length; i++) {
            if (i > 0) script += ",";
            script += args[i];
        }
        script += ")";
        //alert(script);

        if (idx < 0) {
            console.error("Unknow Command \"" + command + "\"");
            script = "NULL";
            dpz.console.lastCommandResult = "未知命令";
            dpz.console.lastErrorCommand = command;
            dpz.console.lastErrorCommandScript = script;
        } else {
            try {
                eval(script);
            } catch (ex) {
                dpz.console.lastCommandResult = ex;
                dpz.console.lastErrorCommand = command;
                dpz.console.lastErrorCommandScript = script;
            }
        }

        if (dpz.console.lastCommandResult === undefined) dpz.console.lastCommandResult = "";

        dpz.console.lastCommand = command;
        dpz.console.lastCommandScript = script;

        return dpz.console.lastCommandResult;
    },

    init: function () {

        var cmds = dpz.console.commands;

        cmds.set("help", "打印所有命令帮助", function () {
            var res = "";
            var idxs = dpz.commands.items.length;
            for (var i = 0; i < idxs; i++) {
                var des = dpz.commands.items[i].description;
                res += dpz.commands.items[i].name;
                if (!dpz.isNull(des.params)) {
                    var args = "";
                    for (var j = 0; j < des.params.length; j++) {
                        var param = "" + des.params[j];
                        if (param !== "") {
                            args += " <" + param + ">";
                        }
                    }
                    res += args;
                }
                res += " - " + des.title;
                if (i < idxs - 1) res += "\n";
            }
            return res;
        });

        cmds.set("log", {
            title: "使用浏览器默认的开发者工具进行调试输出",
            params: ["content"]
        }, function (cnt) { console.log(cnt); });

        cmds.set("get-info", "获取套件信息(返回对象)", function () { return dpz.info; });
        cmds.set("alert", "弹出一个对话框", function (cnt) { alert(cnt); });
        cmds.set("reload", "刷新网页", function () { location.reload(true); });

        cmds.set("goto", {
            title: "跳转网页到新地址",
            params: ["url"]
        }, function (url) {
            if (typeof url !== "string") throw "地址不符合规范";
            location.href = url;
        });

        cmds.set("open", {
            title: "从新的页面打开一个新的地址",
            params: ["url"]
        }, function (url) {
            if (typeof url !== "string") throw "地址不符合规范";
            if (win) win.open(url);
        });

        cmds.set("load-js", {
            title: "动态加载一个js文件",
            params: ["id", "url", "fn"]
        }, dpz.loader.loadJs);

        cmds.set("load-css", {
            title: "动态加载一个css文件",
            params: ["id", "url", "fn"]
        }, dpz.loader.loadCss);

        cmds.set("load-image", {
            title: "动态加载一个css文件",
            params: ["url", "fn", "fnErr"]
        }, dpz.loader.loadImage);

    }
};

/**
 * 大胖子专用动态加载器
 * */
dpz.loader = new DpzDynamicLoader();

/**
 * 大胖子命令管理器
 * */
dpz.commands = dpz.console.commands;

/**
 * 执行大胖子控制台命令
 * @param {string} cmd 命令字符串
 * @returns {any} 命令返回
 */
dpz.exec = function (cmd) {
    return dpz.console.exec(cmd);
};

/**
 * 大胖子套件初始化专用对象
 * */
dpz.initializer = {
    documentIsReady: false,
    domContentLoaded: function () {
        if (typeof document !== "undefined") {
            //取消事件监听，执行ready方法
            if (document.addEventListener) {
                document.removeEventListener("DOMContentLoaded", dpz.initializer.domContentLoaded, false);
                dpz.initializer.documentReady();
            }
            else if (document.readyState === "complete") {
                document.detachEvent("onreadystatechange", dpz.initializer.domContentLoaded);
                dpz.initializer.documentReady();
            }
        }
    },
    documentReady: function () {

        //如果已加载，则退出处理
        if (dpz.initializer.documentIsReady) return;

        //设置已加载标志
        dpz.initializer.documentIsReady = true;
        //tmp.DOMContentLoaded();

        //将dpz加入window对象
        if (typeof window !== "undefined") {
            if (typeof window["dpz"] === "undefined") window.dpz = dpz;
        }

        //进行初始命令设置
        dpz.console.init();

        //执行所有绑定的执行对象
        dpz.initializer.handlersExecute();
    },
    handlers: [],
    handlersExecute: function () {
        var my = this;
        var idx = my.handlers.length;
        for (var i = 0; i < idx; i++) {
            if (typeof my.handlers[i] === "function") my.handlers[i]();
        }
        my.handlers = [];
    }
};

/**
 * 大胖子套件使用入口
 * @param {string} fn 绑定的方法
 * */
dpz.ready = function (fn) {
    var idx = dpz.initializer.handlers.length;
    dpz.initializer.handlers[idx] = fn;
};

//绑定加载事件进行套件初始化
if (typeof document !== "undefined") {
    if (document.addEventListener) {
        // Use the handy event callback
        document.addEventListener("DOMContentLoaded", dpz.initializer.domContentLoaded, false);

        // A fallback to window.onload, that will always work
        if (typeof window !== "undefined") window.addEventListener("load", dpz.initializer.documentReady, false);

        // If IE event model is used
    } else {
        // Ensure firing before onload, maybe late but safe also for iframes
        document.attachEvent("onreadystatechange", dpz.initializer.domContentLoaded);

        // A fallback to window.onload, that will always work
        if (window) window.attachEvent("onload", dpz.initializer.documentReady);
    }
}