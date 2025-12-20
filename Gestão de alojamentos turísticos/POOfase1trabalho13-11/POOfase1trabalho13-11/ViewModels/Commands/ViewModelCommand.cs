using System;
using System.Windows.Input;

namespace POOfase1trabalho13_11.ViewModels.Commands
{
   
    public class ViewModelCommand : ICommand
    {
        #region Delegates
        public delegate void ICommandOnExecute(object? parameter);
        public delegate bool ICommandOnCanExecute(object? parameter);
        #endregion

        #region Fields
        private readonly ICommandOnExecute _execute;
        private readonly ICommandOnCanExecute? _canExecute;
        #endregion

        #region Constructors

        public ViewModelCommand(ICommandOnExecute onExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
        }

        public ViewModelCommand(ICommandOnExecute onExecuteMethod, ICommandOnCanExecute onCanExecuteMethod)
        {
            _execute = onExecuteMethod ?? throw new ArgumentNullException(nameof(onExecuteMethod));
            _canExecute = onCanExecuteMethod ?? throw new ArgumentNullException(nameof(onCanExecuteMethod));
        }

        #endregion

        #region ICommand Members

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public bool CanExecute(object? parameter)
        {
            return _canExecute?.Invoke(parameter) ?? true;
        }

        public void Execute(object? parameter)
        {
            _execute.Invoke(parameter);
        }

        #endregion

        
        public void RaiseCanExecuteChanged()
        {
            CommandManager.InvalidateRequerySuggested();
        }
    }
}