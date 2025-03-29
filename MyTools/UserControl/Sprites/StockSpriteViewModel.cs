using HandyControl.Controls;
using HandyControl.Data;
using HandyControl.Interactivity;
using LiteDB;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using Masuit.Tools;
using MyTools.DTO;
using MyTools.Helpers;
using MyTools.Model;
using MyTools.WebHttp;
using Serilog;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;

namespace MyTools.UserControl.Sprites
{
    public class StockSpriteViewModel : BaseViewModel
    {
        private readonly static IStock _API = HttpDispatchProxy.CreateProxy<IStock>("https://qt.gtimg.cn");
        private Task _updateTask;
        private string _StockCode = "";
        private ObservableCollection<StockInfo> _StockInfos;
        private StockInfo _SelectedStockInfo;
        /// <summary>
        /// 休市状态
        /// </summary>
        private bool StockClosed = false;

        /// <summary>
        /// 股票代码
        /// </summary>
        public string StockCode
        {
            get => _StockCode; set
            {
                _StockCode = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<StockInfo> StockInfos
        {
            get => _StockInfos; set
            {
                _StockInfos = value;
                OnPropertyChanged();
            }
        }
        public StockInfo SelectedStockInfo
        {
            get => _SelectedStockInfo; set
            {
                _SelectedStockInfo = value;
                OnPropertyChanged();
                ObservableValues.Clear();
                if (_updateTask == null || _updateTask.Status == TaskStatus.Faulted) UpdateSeries();
            }
        }

        public ObservableCollection<ISeries> Series { get; set; }

        public ObservableCollection<ObservableValue> ObservableValues { get; set; } = new ObservableCollection<ObservableValue>();

        public ObservableCollection<ICartesianAxis> XAxes { get; set; }

        public StockSpriteViewModel()
        {
            Series = [
                new LineSeries<ObservableValue>{
                    Values = ObservableValues,
                    Fill = new SolidColorPaint(SKColors.Blue.WithAlpha(30)),
                    Stroke = new SolidColorPaint(SKColors.Blue) // 使用 SolidColorPaint
                    {
                        StrokeThickness = 1, // 线条粗细
                    },
                    GeometrySize = 1, // 设置数据点的大小
                    GeometryFill = new SolidColorPaint(SKColors.AliceBlue),
                    LineSmoothness = 0.8, // 曲线的平滑度 (0为折线，1为圆滑曲线)
                }
            ];
            RefreshCurrenInfo();
        }

        #region Command

        public ICommand AddStockInfo => new MyCommand(async o =>
        {
            var http = new HTTPHelper();
            if (StockCode.IsNullOrEmpty())
            {
                MessageBox.Warning("请输入正确的股票代码！");
                return;
            }
            var stockResult = await http.GetAsync<TencentStockDTO>($"https://proxy.finance.qq.com/cgi/cgi-bin/smartbox/search?stockFlag=1&app=official_website&query={StockCode}");
            if (stockResult == null || stockResult.stock == null)
            {
                MessageBox.Error("获取信息异常！");
                return;
            }
            if (stockResult.stock.Count < 1)
            {
                MessageBox.Warning("未查询到该股票信息！");
                return;
            }
            if (stockResult.stock.Count > 1)
            {
                var stockMsg = stockResult.stock.Select(o => $"{o.name}：{o.code}").ToList();
                MessageBox.Warning("股票代码不唯一，请重新输入！\r\n" + string.Join("\r\n", stockMsg));
                return;
            }
            var stockInfo = new StockInfo()
            {
                StockCode = stockResult.stock[0].code,
                StockName = stockResult.stock[0].name
            };
            if (StockInfos.Any(o => o.StockCode == stockInfo.StockCode))
            {
                MessageBox.Warning("该信息已经存在，请勿重复添加！");
                return;
            }
            stockInfo.DBInsert();
            RefreshCurrenInfo();
            StockCode = "";
            MessageBox.Success("添加成功！");
        });
        public ICommand DeleteStockInfo => new MyCommand(o =>
        {
            if (SelectedStockInfo == null || SelectedStockInfo.Id.IsNullOrEmpty())
            {
                MessageBox.Warning("已经没有可以删除的信息了！");
                return;
            }
            var result = MessageBox.Ask("确定删除吗？", "提示");
            if (result == System.Windows.MessageBoxResult.Cancel) return;
            SelectedStockInfo.DBDelete();
            RefreshCurrenInfo();
        });
        public ICommand Close => new MyCommand(o =>
        {
            ControlCommands.CloseWindow.Execute(o);
            base.DisposeContent();
            //var windows = App.Current.Windows.Cast<System.Windows.Window>().Where(o => o.GetType() != typeof(MainWindow) && o.GetType().Name != "AdornerWindow");
            //if (windows.Count() == 0 && App.Current.MainWindow.Visibility == System.Windows.Visibility.Hidden)
            //{
            //    App.Current.Shutdown();
            //}
        });
        #endregion

        private (string, MinList) LastTrendInfo = ("000000", new MinList());
        private void UpdateSeries()
        {
            _updateTask = Task.Run(async () =>
            {
                var _http = new HTTPHelper();
                while (!IsDisposed)
                {
                    try
                    {
                        await Task.Delay(1000 * 5);
                        if (SelectedStockInfo == null || SelectedStockInfo.StockCode.IsNullOrEmpty()) continue;
                        #region 监听股市状态
                        var stockStatus = _API.GetStockStauts(new Dictionary<string, object>
                        {
                            {"q","marketStat" },
                            { "fmt","json"}
                        });
                        if (stockStatus.Contains("已休市"))
                        {
                            StockClosed = true;
                            if (ObservableValues.Any()) continue;
                        }
                        #endregion
                        #region 获取个股信息
                        var stockInfoResult = _API.GetStockInfo(new Dictionary<string, object>
                        {
                            {"q",SelectedStockInfo.StockCode},
                            {"fmt","json"}
                        });
                        var info = stockInfoResult.First().Value;
                        SelectedStockInfo.CurrentPrice = double.Parse(info[3]);
                        SelectedStockInfo.ChangePercentageValue = double.Parse(info[31]);
                        SelectedStockInfo.ChangePercentage = double.Parse(info[32]);
                        OnPropertyChanged(nameof(SelectedStockInfo));
                        #endregion
                        #region 获取个股趋势
                        var trendResult = await _http.GetAsync<TencentStockDTO>($"https://proxy.finance.qq.com/cgi/cgi-bin/fundflow/hsfundtab?code={SelectedStockInfo.StockCode}&type=todayFundTrend");
                        if (trendResult.data.todayFundTrend == null || !trendResult.data.todayFundTrend.minList.Any()) continue;
                        var infoList = trendResult.data.todayFundTrend.minList;
                        var thenTrendInfo = infoList.Last();
                        if (thenTrendInfo.time == LastTrendInfo.Item2.time && SelectedStockInfo.StockCode == LastTrendInfo.Item1) continue;
                        if ((thenTrendInfo.TimeParse() - LastTrendInfo.Item2.TimeParse()).Days > 0) ObservableValues.Clear();
                        var values = infoList.Skip(ObservableValues.Count);
                        ObservableValues.AddRange(values.Select(o => new ObservableValue(o.Price.ToDouble())));
                        LastTrendInfo = (SelectedStockInfo.StockCode, thenTrendInfo);
                        XAxes = new ObservableCollection<ICartesianAxis>
                        {
                            new Axis{
                                Labels = infoList.Select(o => o.TimeParse().ToString("HH:mm")).ToArray(),
                            }
                        };
                        OnPropertyChanged(nameof(XAxes));
                        #endregion
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                }
            });
        }
        /// <summary>
        /// 刷新当前信息
        /// </summary>
        private void RefreshCurrenInfo()
        {
            StockInfos = LiteDBHelper.DBFind<StockInfo>().ToObservableCollection();
            SelectedStockInfo = StockInfos.FirstOrDefault() ?? new StockInfo();
        }
    }
}
