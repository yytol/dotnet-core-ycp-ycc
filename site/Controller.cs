using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Internal;

namespace site {
    public class Controller : dpz.Mvc.Controllers.JttpController {

        //public Controller() : base(new dpz.Mvc.Sessions.RedisSessionManager("127.0.0.1:8379,password=Yunyitong2016")) {
        //    base.Session.Init(this);
        //}

        public Controller() : base(new dpz.Mvc.Sessions.MemorySessionManager()) {
            //base.Session.Init(this);
            //base.Response.Headers.Add("xxx", "123");
        }

        protected override void OnExecuting(ActionExecutingContext context) {
            base.OnExecuting(context);
        }

        protected override void OnExecuted(ActionExecutedContext context) {
            base.OnExecuted(context);
        }

    }
}
