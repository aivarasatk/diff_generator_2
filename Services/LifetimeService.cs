using DiffGenerator2.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiffGenerator2.Services
{
    public class LifetimeService : ILifetimeService
    {
        public void ExecuteInLifetime<TLifeTime>(Action<TLifeTime> action)
        {
            throw new NotImplementedException();
        }

        public T ExecuteInLifetime<T, TLifeTime>(Func<TLifeTime, T> action)
        {
            throw new NotImplementedException();
        }
    }
}
