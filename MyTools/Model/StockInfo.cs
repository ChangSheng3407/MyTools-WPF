using LiteDB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.Model
{
    public class StockInfo
    {
        public ObjectId Id { get; set; }
        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode { get; set; }
        /// <summary>
        /// 股票名称
        /// </summary>
        public string StockName { get; set; }
        /// <summary>
        /// 当前价
        /// </summary>
        public double CurrentPrice { get; set; }
        /// <summary>
        /// 涨跌幅
        /// </summary>
        public double ChangePercentage { get; set; }
        /// <summary>
        /// 涨跌值
        /// </summary>
        public double ChangePercentageValue { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; } = DateTime.Now;
    }
}
