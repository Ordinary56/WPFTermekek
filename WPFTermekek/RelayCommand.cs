using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace WPFTermekek
{
    public class RelayCommand : ICommand
    {
        private readonly Predicate<object> _canexecute;

        private readonly Action<object> _execute;

        public event EventHandler? CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
        
        public RelayCommand(Action<object> execute, Predicate<object> canexecute)
        {
            _execute = execute ?? throw new ArgumentNullException("No executable command was given");
            _canexecute = canexecute;
        }
        public RelayCommand(Action<object> execute) : this(execute,o => true)
        {

        }

        public bool CanExecute(object? parameter)
        {
            return _canexecute(parameter!);
        }

        public void Execute(object? parameter)
        {
            _execute(parameter!);
        }
    }
}
