using HandyControl.Controls;
using HandyControl.Data;
using ICSharpCode.AvalonEdit.Document;
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

namespace MyTools.UserControl.Tools
{
    /// <summary>
    /// StringConcat.xaml 的交互逻辑
    /// </summary>
    public partial class StringConcat
    {
        public StringConcat()
        {
            InitializeComponent();
            DataContext = new StringConcatViewModel();
        }

        private void InputValueChanged(object sender, EventArgs e)
        {
            if (DataContext == null) return;
            var context = (StringConcatViewModel)DataContext;
            var rows = context.InputValue.Text.Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
            context.ResultValue = new TextDocument(string.Join("\r\n", rows.Select(o => context.Prefix + o + context.Stuffix)));
        }
    }
}