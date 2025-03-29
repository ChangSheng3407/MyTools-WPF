using HandyControl.Controls;
using HandyControl.Data;
using LiteDB;
using Masuit.Tools.Hardware;
using Microsoft.Win32;
using MyTools.Helpers;
using MyTools.Model;
using MyTools.UserControl;
using MyTools.UserControl.Sprites;
using MyTools.UserControl.Tools;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MyTools.ViewModel
{
    class MainViewModel : BaseViewModel
    {
        private string _BackgroundPath;
        public string BackgroundPath
        {
            get
            {
                if (string.IsNullOrEmpty(_BackgroundPath))
                {
                    var data = LiteDBHelper.DBFind<SystemConfig>(x => x.Key == "BackgroundPath").FirstOrDefault();
                    _BackgroundPath = data?.Value;
                }
                if (!File.Exists(_BackgroundPath))
                {
                    _BackgroundPath = "./Resources/Background.png";
                }
                return _BackgroundPath;
            }
            set
            {
                _BackgroundPath = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ItemViewModel> DevelopmentItems { get; set; }
        public ObservableCollection<ItemViewModel> LifeItems { get; set; }
        public ObservableCollection<ItemViewModel> OtherItems { get; set; }
        public Dictionary<string, ObservableCollection<ItemViewModel>> TabItems { get; set; }
        //private ItemViewModel _selectedItem = new ItemViewModel { Title = "HomePage", Content = new ToDoList() };
        private ItemViewModel _selectedItem = new ItemViewModel { Title = "HomePage", Content = new HomePageControl() };
        private string _PrivateIPAddress = "Unknown";
        private string _PublicIPAddress = "Unknown";

        public ItemViewModel SelectedItem
        {
            get => _selectedItem;
            set
            {
                if (_selectedItem.Content is FrameworkElement homePage && homePage.DataContext is BaseViewModel viewModel)
                {
                    viewModel.DisposeContent();
                }
                _selectedItem = value;
                if (value.Content == null) value.CreateContent();
                OnPropertyChanged();
            }
        }
        public string PrivateIPAddress
        {
            get => _PrivateIPAddress; set
            {
                _PrivateIPAddress = value;
                OnPropertyChanged();
            }
        }
        public string PublicIPAddress
        {
            get => _PublicIPAddress; set
            {
                _PublicIPAddress = value;
                OnPropertyChanged();
            }
        }

        // 构造函数，初始化 Items 属性
        public MainViewModel()
        {
            DevelopmentItems = new ObservableCollection<ItemViewModel>
            {
                new ItemViewModel {Title="字符拼接",ImageSource="../Resources/Icons/连接.png",TypeFullName=typeof(StringConcat).FullName},
                new ItemViewModel {Title="二维码",ImageSource="../Resources/Icons/二维码.png",TypeFullName=typeof(QRCode).FullName},
                new ItemViewModel {Title="JsonExcel",ImageSource="../Resources/Icons/转换.png",TypeFullName=typeof(JsonExcel).FullName},
                new ItemViewModel {Title="Base64与图片",ImageSource="../Resources/Icons/图片.png",TypeFullName=typeof(Base64Pic).FullName},
                new ItemViewModel {Title="Docker工具",ImageSource="../Resources/Icons/Docker.png",TypeFullName=typeof(DockerTool).FullName}
            };
            LifeItems = new ObservableCollection<ItemViewModel>
            {
                new ItemViewModel {Title = "月度账单汇总",ImageSource="../Resources/Icons/账单.png",TypeFullName=typeof(Bill_Summary).FullName},
                new ItemViewModel {Title = "这个班值不值",ImageSource="../Resources/Icons/休息.png",TypeFullName=typeof(Work_Ratio).FullName},
                new ItemViewModel {Title = "待办清单",ImageSource="../Resources/Icons/待办事项.png",TypeFullName=typeof(ToDoList).FullName},
                new ItemViewModel {Title = "Item 3",ImageSource="../Resources/Icons/账单.png",TypeFullName=typeof(Button).FullName},
            };
            OtherItems = new ObservableCollection<ItemViewModel>
            {
                new ItemViewModel {Title = "右键菜单管理",ImageSource="../Resources/Icons/账单.png",TypeFullName=typeof(Button).FullName},
                new ItemViewModel {Title = "定时任务",ImageSource="../Resources/Icons/定时.png",TypeFullName=typeof(TimeTask).FullName}
            };
            TabItems = new Dictionary<string, ObservableCollection<ItemViewModel>>
            {
                {"开发者", DevelopmentItems },
                {"日常", LifeItems },
                {"其他", OtherItems}
            };
            RefreshIPAddressCommand.Execute(null);
        }
        /// <summary>
        /// 切换背景图片
        /// </summary>
        public ICommand ChangeBackgroundCommand => new MyCommand(o =>
        {
            FileDialog dialog = new OpenFileDialog();
            dialog.Filter = "图片文件(*.jpg;*.png;*.jpeg)|*.jpg;*.png;*.jpeg";
            if (dialog.ShowDialog() == true)
            {
                var data = LiteDBHelper.DBFind<SystemConfig>(x => x.Key == "BackgroundPath").First();
                data.Value = dialog.FileName;
                data.DBUpdate();
                BackgroundPath = dialog.FileName;
            }
        });
        /// <summary>
        /// 刷新IP地址
        /// </summary>
        public ICommand RefreshIPAddressCommand => new MyCommand(o =>
        {
            Task.Run(() =>
            {
                try
                {
                    PrivateIPAddress = SystemInfo.GetLocalUsedIP().ToString();
                    Growl.Success(new GrowlInfo { Message = "获取本地IP成功！", WaitTime = 3 });
                    PublicIPAddress = new HTTPHelper().GetPublicIPAddressAsync();
                    Growl.Success(new GrowlInfo { Message = "获取公网IP成功！", WaitTime = 3 });
                }
                catch (Exception ex)
                {
                    Growl.Error(ex.Message);
                }
            });
        });
        /// <summary>
        /// 清除通知
        /// </summary>
        public ICommand ClearNotificationCommand => new MyCommand(o =>
        {
            CommonHelper.ClearAllGrowlNotifications();
        });
        public ICommand ReturnDetaultPage => new MyCommand(o =>
        {
            if (SelectedItem.Content is HomePageControl)
            {
                return;
            }
            SelectedItem = new ItemViewModel { Title = "HomePage", Content = new HomePageControl() };
        });
        private Sprite sprite = null;
        public ICommand ShowStockSprite => new MyCommand(o =>
        {
            sprite = Sprite.Show(new StockSprite());
            //sprite.Closed += (s, e) => sprite = null;
            //if (sprite == null)
            //{
            //    sprite = Sprite.Show(new StockSprite());
            //    sprite.Closed += (s, e) => sprite = null;
            //}
            //else
            //{
            //    HandyControl.Controls.MessageBox.Success("已经打开了一个盯盘！");
            //}
        });
        /// <summary>
        /// 复制到剪切板
        /// </summary>
        public ICommand CopyToClipboard => new MyCommand(o =>
        {
            CommonHelper.CopyToClipboard(o);
            Growl.Success("已复制到剪贴板");
        });
    }
}
