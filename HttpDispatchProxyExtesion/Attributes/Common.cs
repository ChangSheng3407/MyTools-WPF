using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpDispatchProxyExtesion.Attributes
{
    public class EncodingAttribute : Attribute
    {
        public EncodingAttribute(string encoding)
        {
            Encoding = Encoding.GetEncoding(encoding);
        }

        public Encoding Encoding { get; set; }
    }
}
