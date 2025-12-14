using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Final.ViewModels.Commands
{
    public class AsyncCommand : ICommand
    {
        private readonly Func<object?, Task> _execute;
        private readonly Func<object?, bool>? _canExecute;


        public AsyncCommand(Func<object?, Task> execute, Func<object?, bool>? canExecute)
        {
            _execute = execute ?? throw new NullReferenceException("execute");
            _canExecute = canExecute;
        }


        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }


        public bool CanExecute(object? parameter)
        {
            return _canExecute == null ? true : _canExecute.Invoke(parameter);
        }


        public async void Execute(object? parameter)
        {
            await _execute.Invoke(parameter);
            CommandManager.InvalidateRequerySuggested();
        }
    }
}
