using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiffGenerator2.Interfaces
{
    public interface ICommandFactory
    {
        ICommand CreateCommand(Action executeAction);
        ICommand CreateCommand(Action executeAction, Predicate<object> canExecute);
    }
}
