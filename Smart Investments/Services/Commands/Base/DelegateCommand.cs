using System;
using System.Windows.Input;

namespace Smart_Investments.Services.Commands.Base
{
    internal class DelegateCommand : Command
    {
        private Predicate<object> _canExecute;
        private Action<object> _execute;

        public DelegateCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            this._canExecute = canExecute;
            this._execute = execute;
        }

        public override bool CanExecute(object parameter)
        {
            return this._canExecute == null || this._canExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            this._execute(parameter);
        }
    }
}
