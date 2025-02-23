using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Model
{
    public class SystemConfig
    {
        public ObjectId Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }
    }
}
