using HandyControl.Controls;
using Microsoft.Win32;
using MyTools.Helpers;
using System;
using System.Drawing.Imaging;
using System.Windows;

namespace MyTools.UserControl.Tools
{
    /// <summary>
    /// QRCode.xaml 的交互逻辑
    /// </summary>
    public partial class QRCode
    {
        public QRCode()
        {
            InitializeComponent();
        }

        private void DeQRCode(object sender, System.Windows.RoutedEventArgs e)
        {
            var path = ImgSelector.Uri?.LocalPath;
            if (string.IsNullOrEmpty(path))
            {
                Growl.Error("请先选择文件");
                return;
            }
            var res = QRCodeHelper.DeQrCode(path);
            if (res == null)
            {
                Growl.Warning("解析失败");
            }
            ResultText.Text = res;
        }

        private void CreateQRCode(object sender, System.Windows.RoutedEventArgs e)
        {
            string content = ResultText.Text;
            int width = 100;
            int height = 100;
            int margin = 1;
            if (string.IsNullOrEmpty(content))
            {
                Growl.Error("需要生成的文本不能为空");
                return;
            }
            if (!string.IsNullOrEmpty(ImgWidth.Text) && !int.TryParse(ImgWidth.Text, out width))
            {
                Growl.Error("错误的参数：宽");
                return;
            }
            if (!string.IsNullOrEmpty(ImgHeight.Text) && !int.TryParse(ImgHeight.Text, out height))
            {
                Growl.Error("错误的参数：高");
                return;
            }
            if (!string.IsNullOrEmpty(ImgMargin.Text) && !int.TryParse(ImgMargin.Text, out margin))
            {
                Growl.Error("错误的参数：边距");
                return;
            }
            if (width == 0) width = 100;
            if (height == 0) height = 100;
            string filePath = QRCodeHelper.CreateZXingQrCode(content, width, height, margin);
            var imgBrowser = new ImageBrowser(filePath);
            imgBrowser.Width = 650;
            imgBrowser.Height = 500;
            imgBrowser.Title = "生成的二维码";
            imgBrowser.Show();
            Growl.Success("生成成功");
        }

        private void ImgSelector_Drop(object sender, System.Windows.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 1)
                {
                    throw new Exception("只允许添加一个文件");
                }
                string file = files[0];
                CommonHelper.SetImageSelectorUri(ImgSelector, file);
            }
        }
    }
}