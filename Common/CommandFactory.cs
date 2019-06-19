using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DiffGenerator2.Common
{
    public class CommandFactory : ICommandFactory
    {
        public ICommand CreateCommand(Action executeAction)
        {
            return new Command(executeAction);
        }

        public ICommand CreateCommand(Action executeAction, Predicate<object> canExecute)
        {
            throw new NotImplementedException();
        }
    }
}
