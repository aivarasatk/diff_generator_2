using Autofac;
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
        private readonly ILifetimeScope _lifetimeScope;
        private readonly ILogService _logService;

        public LifetimeService(ILifetimeScope lifetimeScope, ILogService logService)
        {
            _lifetimeScope = lifetimeScope;
            _logService = logService;
        }
        public void ExecuteInLifetime<TLifeTime>(Action<TLifeTime> action)
        {
            try
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    action(scope.Resolve<TLifeTime>());
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Exception during void lifetime execution", ex);
                throw;
            }
            
        }

        public T ExecuteInLifetime<T, TLifeTime>(Func<TLifeTime, T> action)
        {
            try
            {
                using (var scope = _lifetimeScope.BeginLifetimeScope())
                {
                    return action(scope.Resolve<TLifeTime>());
                }
            }
            catch (Exception ex)
            {
                _logService.Error("Exception during value lifetime execution", ex);
                throw;
            }
        }
    }
}
