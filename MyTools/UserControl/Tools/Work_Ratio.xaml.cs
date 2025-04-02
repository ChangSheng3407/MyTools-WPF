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
    /// Work_Ratio.xaml 的交互逻辑
    /// </summary>
    public partial class Work_Ratio
    {
        public Work_Ratio()
        {
            InitializeComponent();
            DataContext = new Work_RatioViewModel();
        }
    }
}
