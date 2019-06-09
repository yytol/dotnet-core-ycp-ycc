using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Kernel.Wss {
    public interface IRequestResult {
        void SetResult(dpz.Jsons.Jttp jttp);
    }
}
