using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface ILogService
    {
        void Information(string message);
        void Information(string message, params object [] values);
        void Information(string message, Exception ex);


        void Error(string message);
        void Error(string message, params object[] values);
        void Error(string message, Exception ex);
    }
}
