using HttpDispatchProxyExtension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.WebHttp
{
    public interface IYiYan : IHttpDispatchProxy
    {
        [GET]
        public Dictionary<string, string> GetResult();
    }
}
