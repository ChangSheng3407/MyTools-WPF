using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Model
{
    public class BillData
    {
        /// <summary>
        /// 交易日期
        /// </summary>
        public DateTime? TransactionDate { get; set; }
        /// <summary>
        /// 交易描述
        /// </summary>
        public string Describe { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public float amount { get; set; }
    }
}
