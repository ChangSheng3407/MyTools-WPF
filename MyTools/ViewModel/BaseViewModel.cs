using HandyControl.Controls;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
namespace MyTools.ViewModel
{
    public class BaseViewModel : INotifyPropertyChanged
    {
        private object? _content;
        public object? Content
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MyProperty { get; set; }
        public string TypeFullName { get; set; }

        public void CreateContent()
        {
            try
            {
                var type = Type.GetType(TypeFullName);
                if (type == null)
                {
                    Growl.Error("错误的组件");
                    return;
                }
                Content = Activator.CreateInstance(type);
            }
            catch (Exception ex)
            {
                Growl.Error($"创建内容时发生错误: {ex.Message}");
            }
        }

        public bool IsDisposed = false;

        public virtual void DisposeContent()
        {
            // 释放资源
            Content = null;
            IsDisposed = true;
            GC.Collect();
        }

        // 属性更改事件通知
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class MyCommand : ICommand
    {
        private readonly Action<object> _execAction;
        private readonly Func<object, bool>? _changeFunc;

        public MyCommand(Action<object> execAction, Func<object, bool>? changeFunc = null)
        {
            _execAction = execAction;
            _changeFunc = changeFunc;
        }

        public event EventHandler? CanExecuteChanged;

        public bool CanExecute(object? parameter)
        {
            if (_changeFunc == null)
            {
                return true;
            }
            return _changeFunc.Invoke(parameter);
        }

        public void Execute(object? parameter)
        {
            _execAction.Invoke(parameter);
        }
    }
}