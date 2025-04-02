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

namespace MyTools.UserControl.MiniControl
{
    /// <summary>
    /// ImgMiniControl.xaml 的交互逻辑
    /// </summary>
    public partial class ImgMiniControl
    {
        private ImgMiniControl()
        {
        }

        /// <summary>
        /// https://dayu.qqsuu.cn/moyuribao/apis.php
        /// https://dayu.qqsuu.cn/moyuribaoshipin/apis.php
        /// https://dayu.qqsuu.cn/mingxingbagua/apis.php
        /// https://dayu.qqsuu.cn/moyurili/apis.php
        /// </summary>
        /// <param name="imgUrl"></param>
        public ImgMiniControl(string imgUrl)
        {
            DataContext = imgUrl;
            InitializeComponent();
        }
    }
}
