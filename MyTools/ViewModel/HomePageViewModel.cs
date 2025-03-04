using HandyControl.Controls;
using Masuit.Tools;
using MyTools.Helpers;
using MyTools.UserControl.MiniControl;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTools.ViewModel
{
    public class HomePageViewModel : BaseViewModel
    {
        private string _YiYan = "Helllo World!";
        private string _YiYanAuthor = "-- C#";

        public string YiYan
        {
            get => _YiYan; set
            {
                _YiYan = value;
                OnPropertyChanged();
            }
        }
        public string YiYanAuthor
        {
            get => _YiYanAuthor; set
            {
                _YiYanAuthor = value;
                OnPropertyChanged();
            }
        }
        public CancellationTokenSource? YiYancts { get; set; }

        public HomePageViewModel()
        {
            YiYanTask();
        }

        private void YiYanTask()
        {
            YiYancts = new CancellationTokenSource();
            Task.Run(async () =>
            {
                var http = new HTTPHelper();
                while (!IsDisposed)
                {
                    try
                    {
                        YiYancts.Token.ThrowIfCancellationRequested();
                        var yiyanAPI = await http.GetAsync<Dictionary<string, string>>("https://v1.hitokoto.cn/");
                        YiYan = yiyanAPI["hitokoto"];
                        YiYanAuthor = $"-- {yiyanAPI["from"]} {yiyanAPI["from_who"]}";
                        if (yiyanAPI["from"] == yiyanAPI["from_who"] || yiyanAPI["from_who"].IsNullOrEmpty())
                        {
                            YiYanAuthor = $"-- {yiyanAPI["from"]}";
                        }
                        await Task.Delay(30 * 1000);
                    }
                    catch (OperationCanceledException)
                    {
                        break;
                    }
                    catch (Exception)
                    {
                        Growl.Error("获取失败，请检查网络连接！");
                        break;
                    }
                }
            }, YiYancts.Token);
        }

        public override void DisposeContent()
        {
            if (YiYancts != null) YiYancts.Cancel();
            base.DisposeContent();
        }
        public ICommand OpenImgMiniControl => new MyCommand(o =>
        {
            Dialog.Show(new ImgMiniControl(o.ToString()));
        });

        public System.Windows.DragEventHandler DragEnterHandler => new System.Windows.DragEventHandler((sender, e) =>
        {
            MessageBox.Show("开始拖动文件！");
        });
    }
}
