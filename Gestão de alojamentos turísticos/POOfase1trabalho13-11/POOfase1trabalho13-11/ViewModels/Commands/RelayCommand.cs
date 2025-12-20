using System;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels.Commands
{
    public class RelayCommand<T> : ICommand
    {
        private readonly Action<T> _execute;

        public RelayCommand(Action<T> execute)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        }

        public bool CanExecute(object? parameter) => true;

       
        

        public void Execute(object? parameter)
        {
            if (parameter is T value)
            {
                _execute(value);
            }
            else if (parameter == null && !typeof(T).IsValueType)
            {
                _execute(default!);
            }
        }
        
        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
} 