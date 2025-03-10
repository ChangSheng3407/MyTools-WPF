using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace MyTools.ViewModel
{
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
