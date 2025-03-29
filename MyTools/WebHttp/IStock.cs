using HttpDispatchProxyExtesion.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.WebHttp
{
    public interface IStock : IHttpDispatchProxy
    {
        [GET]
        [Encoding("GBK")]
        public string GetStockStauts([Query] Dictionary<string, object> query);
        [GET]
        [Encoding("GBK")]
        public Dictionary<string, string[]> GetStockInfo([Query] Dictionary<string, object> query);
    }
}
