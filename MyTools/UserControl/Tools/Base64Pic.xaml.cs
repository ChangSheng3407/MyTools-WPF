using HandyControl.Controls;
using Masuit.Tools;
using MyTools.Helpers;
using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;

namespace MyTools.UserControl.Tools
{
    /// <summary>
    /// Base64Pic.xaml 的交互逻辑
    /// </summary>
    public partial class Base64Pic
    {
        public Base64Pic()
        {
            InitializeComponent();
        }

        private void EncodeBtn_Click(object sender, RoutedEventArgs e)
        {
            if (PicSource.Uri == null || PicSource.Uri.LocalPath.IsNullOrEmpty())
            {
                Growl.Warning("请选择图片");
                return;
            }
            var str = Base64PicHelper.Pic_Base64(PicSource.Uri.LocalPath);
            Base64Str.Text = str;
        }

        private void DecodeBtn_Click(object sender, RoutedEventArgs e)
        {
            var str = Base64Str.Text;
            if (string.IsNullOrEmpty(str)) return;
            var tempFile = Base64PicHelper.Base64_Pic(str);
            if (tempFile != null)
            {
                tempFile = System.IO.Path.Combine(CommonHelper.CurrectDir, tempFile);
                PicSource.SetValue(Image.SourceProperty, new BitmapImage(new Uri(tempFile)));
                Task.Run(() =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        new ImageBrowser(tempFile).ShowDialog();
                    });
                });
                return;
            }
            Growl.Error("错误的Base64字符串");
        }

        private void ClearBtn_Click(object sender, RoutedEventArgs e)
        {
            Base64Str.Clear();
        }

        private void PicSource_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1)
                {
                    throw new Exception("只允许添加一个文件");
                }
                string file = files[0];
                CommonHelper.SetImageSelectorUri(PicSource, file);
            }
        }
    }
}
