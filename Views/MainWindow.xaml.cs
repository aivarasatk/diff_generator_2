using Autofac;
using DiffGenerator2.Common;
using DiffGenerator2.Interfaces;
using DiffGenerator2.ViewModel;
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
            var configuration = ContainerConfig.Configure();
            var mainViewModel = configuration.Resolve<MainViewModel>();
            DataContext = mainViewModel;
        }
    }
}
