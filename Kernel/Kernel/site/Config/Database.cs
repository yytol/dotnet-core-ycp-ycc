using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dpz;

namespace site.Config {
    public class Database {

        private static dpz.Database dbDefend = null;
        private static dpz.Database dbAos = null;
        private static dpz.Database dbManage = null;

        /// <summary>
        /// 获取维护数据库定义
        /// </summary>
        public static dpz.Database Defend {
            get {
                if (dbDefend == null) {
                    //取配置根下的 Database 部分
                    var snDatabase = site.ConfigManager.Root.GetSection("Database");
                    //取配置根下的 Type 部分
                    var snType = snDatabase.GetSection("Type").Value;

                    switch (snType.ToLower()) {
                        case "mysql":
                            dbDefend = new dpz.Gdbc.Databases.MySql() {
                                Address = snDatabase.GetSection("Address").Value,
                                Name = "mysql",
                                Password = snDatabase.GetSection("Password").Value,
                                Port = snDatabase.GetSection("Port").Value.ToInteger(),
                                User = snDatabase.GetSection("User").Value
                            };
                            break;
                        default: throw new Exception($"尚未支持\"{snType}\"类型数据库");
                    }

                }
                return dbDefend;
            }
        }

        /// <summary>
        /// 获取基础库配置
        /// </summary>
        public static dpz.Database Aos {
            get {
                if (dbAos == null) {
                    var defend = Database.Defend;
                    switch (defend.Type) {
                        case DatabaseTypes.MySQL:
                            var mysql = defend as dpz.Gdbc.Databases.MySql;
                            dbAos = new dpz.Gdbc.Databases.MySql() {
                                Address = mysql.Address,
                                Name = "Aos",
                                Password = mysql.Password,
                                Port = mysql.Port,
                                User = mysql.User
                            };
                            break;
                        default: throw new Exception($"尚未支持\"{defend.Type.ToString()}\"类型数据库");
                    }
                }
                return dbAos;
            }
        }

        /// <summary>
        /// 获取基础库配置
        /// </summary>
        public static dpz.Database Manage {
            get {
                if (dbManage == null) {
                    var defend = Database.Defend;
                    switch (defend.Type) {
                        case DatabaseTypes.MySQL:
                            var mysql = defend as dpz.Gdbc.Databases.MySql;
                            dbManage = new dpz.Gdbc.Databases.MySql() {
                                Address = mysql.Address,
                                Name = "Aos_Manage",
                                Password = mysql.Password,
                                Port = mysql.Port,
                                User = mysql.User
                            };
                            break;
                        default: throw new Exception($"尚未支持\"{defend.Type.ToString()}\"类型数据库");
                    }
                }
                return dbManage;
            }
        }

    }
}
