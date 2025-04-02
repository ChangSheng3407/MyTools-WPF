using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyTools.ViewModel
{
    class ItemViewModel : BaseViewModel
    {
        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                _title = value;
                OnPropertyChanged();
            }
        }
        private string _imagesource;
        public string ImageSource
        {
            get => _imagesource;
            set
            {
                _imagesource = value;
                OnPropertyChanged();
            }
        }
    }
}
