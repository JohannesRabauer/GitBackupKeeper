using System;
using System.Diagnostics;
using System.Windows.Input;

namespace GitBackupKeeper.Helper
{
    public class RelayCommand : ICommand
    {
        #region Fields 
        readonly Action<object> _executeWithObject;
        readonly Action _executeWithoutObject;
        readonly Predicate<object> _canExecute;
        #endregion // Fields 
        #region Constructors 
        public RelayCommand(Action execute)
        {
            _executeWithoutObject = execute;
        }
        public RelayCommand(Action<object> execute) : this(execute, null) { }
        public RelayCommand(Action<object> execute, Predicate<object> canExecute)
        {
            if (execute == null)
                throw new ArgumentNullException("execute");
            _executeWithObject = execute; _canExecute = canExecute;
        }
        #endregion // Constructors 
        #region ICommand Members 
        [DebuggerStepThrough]
        public bool CanExecute(object parameter)
        {
            return _canExecute == null ? true : _canExecute(parameter);
        }
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public void Execute(object parameter)
        {
            if (_executeWithObject == null)
                _executeWithoutObject();
            else
                _executeWithObject(parameter);
        }
        #endregion // ICommand Members 
    }

}
