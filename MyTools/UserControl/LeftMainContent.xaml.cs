using MyTools.ViewModel;
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

namespace MyTools.UserControl
{
    /// <summary>
    /// LeftMainContent.xaml 的交互逻辑
    /// </summary>
    public partial class LeftMainContent
    {
        public LeftMainContent()
        {
            InitializeComponent();
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var dc = DataContext as MainViewModel;
            var lb = sender as ListBox;
            var im = lb.SelectedValue as ItemViewModel;
            if (im == null) return;
            dc.SelectedItem = im;
        }
    }
}
