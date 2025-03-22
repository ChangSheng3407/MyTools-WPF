using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.DTO
{
    internal class Bill_Summary_Groupby_DTO
    {
        [ExcelColumnWidth(50)]
        public string 交易描述 { get; set; }
        [ExcelColumnWidth(10)]
        public int 交易计数 { get; set; }
        [ExcelColumnWidth(15)]
        public float 交易金额汇总 { get; set; }
        [ExcelColumnWidth(10)]
        public string 消费占比 { get; set; }
    }
}
