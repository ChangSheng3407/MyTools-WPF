using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.DTO
{
    public class TencentStockDTO
    {
        /// <summary>
        /// 
        /// </summary>
        public int code { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string msg { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Data data { get; set; }
        public List<Stock> stock { get; set; }
    }

    public class Summary
    {
        /// <summary>
        /// 
        /// </summary>
        public string mcRatio { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rank { get; set; }
        /// <summary>
        /// 主力净流入额市场排名4834/5382，占流通市值比例0.00%；今日净流入较近5日净流入的均值增加15139.90万元。
        /// </summary>
        public string s0 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v0 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v3 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v4 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string v5 { get; set; }
    }

    public class TodayFundFlow
    {
        /// <summary>
        /// 逐笔统计当日成交买卖单，把资金划分为主力和散户，其中主力=超大单+大单。计算公式为：成交金额大于等于20万元或者大于等于6万股则被判断为主力资金。
        /// </summary>
        public string desc { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainNetIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainInRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainOut { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainOutRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string retailIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string retailInRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string retailOut { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string retailOutRate { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string superFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string bigFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string normalFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string smallFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string rank { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Summary summary { get; set; }
    }

    public class MinList
    {
        /// <summary>
        /// 
        /// </summary>
        public string time { get; set; } = DateTime.MinValue.ToString("yyyyMMddHHmm");
        /// <summary>
        /// 
        /// </summary>
        public DateTime TimeParse() => DateTime.ParseExact(time, "yyyyMMddHHmm", CultureInfo.InvariantCulture);
        /// <summary>
        /// 
        /// </summary>
        public string Price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MainNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RetailNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SuperNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string BigNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string NormalNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string SmallNetInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MainInflow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string MainOutflow { get; set; }
    }

    public class TodayFundTrend
    {
        /// <summary>
        /// 
        /// </summary>
        public string stockCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<MinList> minList { get; set; }
    }

    public class DayMainNetInList
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainNetIn { get; set; }
    }

    public class FiveDayFundFlow
    {
        /// <summary>
        /// 
        /// </summary>
        public string fiveDayMainNetIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public List<DayMainNetInList> DayMainNetInList { get; set; }
    }

    public class OneDayKlineList
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime date { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string mainNetIn { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string price { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string avgIn { get; set; }
    }

    public class HistoryFundFlow
    {
        /// <summary>
        /// 
        /// </summary>
        public List<OneDayKlineList> oneDayKlineList { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public Summary summary { get; set; }
    }

    public class Data
    {
        /// <summary>
        /// 
        /// </summary>
        public TodayFundFlow todayFundFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public TodayFundTrend todayFundTrend { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public FiveDayFundFlow fiveDayFundFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public HistoryFundFlow historyFundFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string activeFlow { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string prec { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string insCode { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string ffHide { get; set; }
    }
    public class Stock
    {
        public string code { get; set; }
        public string name { get; set; }
        public string type { get; set; }
    }
}
