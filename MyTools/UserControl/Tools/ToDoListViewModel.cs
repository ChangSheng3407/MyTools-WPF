using HandyControl.Controls;
using LiteDB;
using Masuit.Tools;
using MyTools.Helpers;
using MyTools.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTools.UserControl.Tools
{
    public class ToDoListViewModel : BaseViewModel
    {
        private bool _DrawerIsOpen;
        private ToDoInfo _CurrectToDo;
        private bool _DrawerIsEnabled;
        private string _DrawerHeaderTitle;
        private ICommand _SaveCommand;

        public string DrawerHeaderTitle
        {
            get => _DrawerHeaderTitle; set
            {
                _DrawerHeaderTitle = value;
                OnPropertyChanged();
            }
        }
        public bool DrawerIsEnabled
        {
            get => _DrawerIsEnabled; set
            {
                _DrawerIsEnabled = value;
                OnPropertyChanged();
            }
        }
        public bool DrawerIsOpen
        {
            get => _DrawerIsOpen; set
            {
                _DrawerIsOpen = value;
                OnPropertyChanged();
            }
        }
        public ToDoInfo CurrectToDo
        {
            get => _CurrectToDo; set
            {
                _CurrectToDo = value;
                OnPropertyChanged();
            }
        }
        public ObservableCollection<ToDoInfo> DataList { get; set; } = new ObservableCollection<ToDoInfo>();
        public ToDoListViewModel()
        {
            RefreshCommand.Execute(null);
        }
        #region Command

        public ICommand RefreshCommand => new MyCommand(o =>
        {
            DataList = LiteDBHelper.DBFind<ToDoInfo>().ToObservableCollection();
            OnPropertyChanged(nameof(DataList));
        });

        public ICommand AddCommand => new MyCommand(o =>
        {
            if (DrawerIsOpen)
            {
                CurrectToDo.DBInsert();
                Growl.Success("添加成功");
                RefreshCommand.Execute(null);
                DrawerIsOpen = false;
                return;
            }
            DrawerIsOpen = true;
            DrawerHeaderTitle = "新增";
            CurrectToDo = new ToDoInfo();
            DrawerIsEnabled = true;
            _SaveCommand = AddCommand;
        });

        public ICommand DelCommand => new MyCommand(o =>
        {
            var msgResult = MessageBox.Show("确认删除？", "提示", System.Windows.MessageBoxButton.OKCancel);
            if (msgResult == System.Windows.MessageBoxResult.OK && o is ToDoInfo info)
            {
                info.DBDelete();
                Growl.Success("删除成功");
                RefreshCommand.Execute(null);
            }
        });

        public ICommand EditCommand => new MyCommand(o =>
        {
            if (DrawerIsOpen)
            {
                CurrectToDo.DBUpdate();
                Growl.Success("修改成功");
                RefreshCommand.Execute(null);
                DrawerIsOpen = false;
                return;
            }
            DrawerIsOpen = true;
            DrawerHeaderTitle = "编辑";
            CurrectToDo = (o as ToDoInfo).DeepClone();
            DrawerIsEnabled = true;
            _SaveCommand = EditCommand;
        });

        public ICommand ViewCommand => new MyCommand(o =>
        {
            if (DrawerIsOpen)
            {
                DrawerIsOpen = false;
                return;
            }
            DrawerIsOpen = true;
            DrawerHeaderTitle = "查看";
            CurrectToDo = (o as ToDoInfo).DeepClone();
            DrawerIsEnabled = false;
            _SaveCommand = ViewCommand;
        });

        public ICommand SaveCommand => new MyCommand(o =>
        {
            _SaveCommand.Execute(null);
        });

        #endregion
    }
}