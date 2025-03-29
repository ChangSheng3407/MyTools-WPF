using HandyControl.Controls;
using HandyControl.Data;
using MiniExcelLibs;
using MyTools.DTO;
using MyTools.Model;
using MyTools.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace MyTools.UserControl.Tools
{
    /// <summary>
    /// Bill_Summary.xaml 的交互逻辑
    /// </summary>
    public partial class Bill_Summary
    {
        public Bill_Summary()
        {
            InitializeComponent();
        }

        private void Start_Click(object sender, RoutedEventArgs e)
        {
            string[] textArray = this.textEditor.Document.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            if (textArray.Length == 0)
            {
                Growl.Error("错误的参数");
                return;
            }
            List<BillData> BillData_List = new List<BillData>();
            foreach (string text in textArray)
            {
                try
                {
                    string[] tempArray = text.Split(new[] { "", "\t" }, StringSplitOptions.RemoveEmptyEntries);
                    if (tempArray[1].Contains("还款"))
                    {
                        continue;
                    }
                    var _ = new BillData
                    {
                        TransactionDate = DateTime.Parse(tempArray[0]),
                        Describe = tempArray[1],
                        Type = "消费",
                        amount = float.Parse(tempArray[2])
                    };
                    if (_.amount < 0) _.Type = "退货";
                    BillData_List.Add(_);
                }
                catch (Exception ex)
                {
                    Growl.Error($"错误行内容【{text}】,请按【交易日期 交易描述 交易金额】格式录入");
                    return;
                }
            }
            BillData_List = BillData_List.OrderByDescending(o => Math.Abs(o.amount)).ToList();
            var sumAmount = BillData_List.Sum(o => o.amount);
            var GroupByDescribe = BillData_List.GroupBy(o => o.Describe, (a, b) => new Bill_Summary_Groupby_DTO
            {
                交易描述 = a,
                交易计数 = b.Count(),
                交易金额汇总 = b.Sum(o => o.amount),
                消费占比 = (b.Sum(o => o.amount) / sumAmount * 100).ToString("0.00") + "%"
            }).OrderByDescending(o => o.交易金额汇总).ToList();
            var sheet_BillDataList = BillData_List.Select(o => new Bill_Summary_DTO
            {
                交易日期 = o.TransactionDate,
                交易描述 = o.Describe,
                交易类型 = o.Type,
                交易金额 = o.amount
            }).ToList();
            sheet_BillDataList.Add(new Bill_Summary_DTO
            {
                交易描述 = "总额",
                交易金额 = sumAmount
            });
            GroupByDescribe.Add(new Bill_Summary_Groupby_DTO
            {
                交易描述 = "总额",
                交易金额汇总 = sumAmount
            });
            var sheets = new Dictionary<string, object>
            {
                {"基础数据明细",sheet_BillDataList },
                {"根据描述汇总",GroupByDescribe }
            };
            string startDate = BillData_List.Select(o => o.TransactionDate).Min().Value.ToString("MM.dd");
            string endDate = BillData_List.Select(o => o.TransactionDate).Max().Value.ToString("MM.dd");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            MiniExcel.SaveAsAsync(Path.Combine(desktopPath, startDate + "-" + endDate + ".xlsx"), sheets);
            Growl.Success("导出成功，请前往桌面查看");
        }
    }
}
