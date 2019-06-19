using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Interfaces
{
    public interface ILifetimeService
    {
        void ExecuteInLifetime<TLifeTime>(Action<TLifeTime> action);
        T ExecuteInLifetime<T, TLifeTime>(Func<TLifeTime, T> action);
    }
}
