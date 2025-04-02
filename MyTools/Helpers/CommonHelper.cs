using HandyControl.Controls;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace MyTools.Helpers
{
    public static class CommonHelper
    {
        private static string _tempDir = "temp";
        /// <summary>
        /// 当前目录
        /// </summary>
        public static string CurrectDir => Directory.GetCurrentDirectory();
        /// <summary>
        /// 临时目录
        /// </summary>
        public static string TempDir
        {
            get
            {
                if (!Directory.Exists("temp"))
                {
                    Directory.CreateDirectory(_tempDir);
                }
                return _tempDir;
            }
        }
        /// <summary>
        /// 桌面目录
        /// </summary>
        public static string desktopPath => Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory);
        /// <summary>
        /// 运行CMD命令
        /// </summary>
        /// <param name="commmand">命令</param>
        /// <returns></returns>
        public static string RunCMD(string commmand)
        {
            string output = "";
            using (Process process = new Process())
            {
                process.StartInfo.FileName = "cmd.exe";
                process.StartInfo.Arguments = "/C " + commmand;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;
                process.Start();
                output = process.StandardOutput.ReadToEnd();
                output = Regex.Unescape(output);
            }
            return output;
        }
        /// <summary>
        /// 清除所有Growl通知
        /// </summary>
        public static void ClearAllGrowlNotifications()
        {
            App.Current.Dispatcher.Invoke(() =>
            {
                Growl.Clear();
                Growl.ClearGlobal();
            });
        }
        public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> source)
        {
            return new ObservableCollection<T>(source);
        }
        /// <summary>
        /// 复制内容到剪贴板
        /// </summary>
        /// <param name="text"></param>
        public static void CopyToClipboard(object text)
        {
            Clipboard.SetDataObject(text);
        }
        /// <summary>
        /// 设置图片选择器Uri
        /// </summary>
        /// <param name="path"></param>
        public static void SetImageSelectorUri(ImageSelector ImgSelector, string path)
        {
            ImgSelector.SetValue(ImageSelector.UriPropertyKey, new Uri(path));
            ImgSelector.SetValue(ImageSelector.PreviewBrushPropertyKey, new ImageBrush(BitmapFrame.Create(ImgSelector.Uri, BitmapCreateOptions.IgnoreImageCache, BitmapCacheOption.None))
            {
                Stretch = ImgSelector.Stretch
            });
            ImgSelector.SetValue(ImageSelector.HasValuePropertyKey, true);
        }
    }
}
