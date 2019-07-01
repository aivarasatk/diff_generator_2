using Autofac;
using DiffGenerator2.Interfaces;
using DiffGenerator2.Services;
using DiffGenerator2.ViewModel;
using Serilog;
using System.Windows.Input;

namespace DiffGenerator2.Common
{
    public class ContainerConfig
    {
        public static IContainer Configure()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<MainViewModel>().SingleInstance();

            builder.RegisterType<EipReader>().As<IEipReader>().InstancePerDependency();
            builder.RegisterType<ExcelReader>().As<IExcelReader>().InstancePerDependency();
            builder.RegisterType<DiffGenerator>().As<IDiffGenerator>().InstancePerDependency();
            builder.RegisterType<CommandFactory>().As<ICommandFactory>().InstancePerDependency();
            builder.RegisterType<Command>().As<ICommand>().InstancePerDependency();

            builder.RegisterType<LifetimeService>().As<ILifetimeService>().SingleInstance();

            var logger = CreateLogger();
            builder.RegisterInstance(logger).As<ILogger>();
            builder.RegisterType<LogService>().As<ILogService>().SingleInstance();

            return builder.Build();
        }

        private static ILogger CreateLogger()
        {
            return LoggerBuilder.Build();
        }
    }
}
