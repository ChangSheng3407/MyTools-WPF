using MiniExcelLibs.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.DTO
{
    internal class Bill_Summary_DTO
    {
        [ExcelColumnWidth(10)]
        public DateTime? 交易日期 { get; set; }
        [ExcelColumnWidth(50)]
        public string 交易描述 { get; set; }
        [ExcelColumnWidth(10)]
        public string 交易类型 { get; set; }
        [ExcelColumnWidth(10)]
        public float 交易金额 { get; set; }
    }
}
