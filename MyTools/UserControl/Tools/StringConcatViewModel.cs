using ICSharpCode.AvalonEdit.Document;
using System.Windows.Input;

namespace MyTools.UserControl.Tools
{
    public class StringConcatViewModel : BaseViewModel
    {
        private IDocument _ResultValue = new TextDocument();
        private string _Prefix = "'";
        private string _Stuffix = "'";
        private IDocument _InputValue = new TextDocument();

        public IDocument ResultValue
        {
            get => _ResultValue; set
            {
                _ResultValue = value;
                OnPropertyChanged();
            }
        }
        public string Prefix
        {
            get => _Prefix; set
            {
                _Prefix = value;
                OnPropertyChanged();
            }
        }
        public string Stuffix
        {
            get => _Stuffix; set
            {
                _Stuffix = value;
                OnPropertyChanged();
            }
        }
        public IDocument InputValue
        {
            get => _InputValue; set
            {
                _InputValue = value;
                OnPropertyChanged();
            }
        }
        public ICommand Clear => new MyCommand(ClearAction);

        public void ClearAction(object parameter)
        {
            InputValue = new TextDocument();
            Prefix = "";
            Stuffix = "";
        }
    }
}
