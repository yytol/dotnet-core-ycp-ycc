using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dpz;

namespace site.Config {
    public class Security {

        /// <summary>
        /// 密码加密算法专用密钥
        /// </summary>
        public const string Password_Key = "swp8whecht3u2xbew9i7q47rk9emk4brljoch4sr97q77elnfyzk2rau36u1rr3suhfee1f6f3b3x05yurweq6wt52koga0v3gjigudslf3nyus300s01kwqt86qq4o3";

        /// <summary>
        /// 获取加密后的密码串
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        public static string GetEncryptionPasswordString(string name, string pwd) {
            string str = pwd + name + Password_Key;
            return "$" + str.GetMD5();
        }

    }
}
