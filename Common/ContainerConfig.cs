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

            builder.RegisterType<EipReader>().As<IEipReader>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelReader>().As<IExcelReader>().InstancePerLifetimeScope();
            builder.RegisterType<DiffGenerator>().As<IDiffGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<CommandFactory>().As<ICommandFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ExcelReportGenerator>().As<IExcelReportGenerator>().InstancePerLifetimeScope();
            builder.RegisterType<FileSelector>().As<IFileSelector>().InstancePerLifetimeScope();

            builder.RegisterType<Command>().As<ICommand>().InstancePerLifetimeScope();

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
