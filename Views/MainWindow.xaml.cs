using Autofac;
using DiffGenerator2.Common;
using DiffGenerator2.Interfaces;
using DiffGenerator2.ViewModel;
using System;
using System.Windows;

namespace DiffGenerator2.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalExceptionHandler);
            var configuration = ContainerConfig.Configure();
            var mainViewModel = configuration.Resolve<MainViewModel>();
            DataContext = mainViewModel;
        }

        private static void GlobalExceptionHandler(object sender, UnhandledExceptionEventArgs args)
        {
            Exception ex = (Exception)args.ExceptionObject;
            MessageBox.Show($"Įvyko nenumatyta klaida, programa bus išjungta.{Environment.NewLine}{ex}", "Globali klaida", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
