using MyTools.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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

namespace MyTools
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            //var windows = App.Current.Windows.OfType<System.Windows.Window>().Where(o => o.GetType().Name != "AdornerWindow").ToList();
            //if (windows.Count > 1)
            //{
            //    e.Cancel = true;
            //    this.Hide();
            //}
            //base.OnClosing(e);
            e.Cancel = true;
            this.Hide();
            base.OnClosing(e);
        }
    }
}
