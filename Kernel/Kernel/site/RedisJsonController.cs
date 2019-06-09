using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace site {
    public class RedisJsonController : dpz.Mvc.Controllers.JsonController {

        public RedisJsonController() : base(new dpz.Mvc.Sessions.RedisSessionManager(site.Config.Redis.ConnectionString)) { }

    }
}
