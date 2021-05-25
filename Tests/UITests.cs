using System;
using System.Collections.Generic;
using System.Threading;
using DiffGenerator2.Common;
using DiffGenerator2.Interfaces;
using DiffGenerator2.ViewModel;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Windows;

namespace Tests
{
    [TestClass]
    public class UITests
    {
        [TestMethod]
        public void When_UI_is_created_spinner_is_disabled()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var commandService = new CommandFactory();
            var lifetimeService = new Mock<ILifetimeService>();
            var orderDiffGenerator = new Mock<IOrderDiffGenerator>();

            //assess
            var mainViewModel = new MainViewModel(commandService, logService.Object, lifetimeService.Object, orderDiffGenerator.Object);

            //assert
            Assert.IsTrue(mainViewModel.Model.IsLoading == Visibility.Collapsed);
        }

        [TestMethod]
        public void When_UI_is_created_sheet_selection_is_disabled()
        {
            //arrange
            var logService = new Mock<ILogService>();
            var commandService = new CommandFactory();
            var lifetimeService = new Mock<ILifetimeService>();
            var orderDiffGenerator = new Mock<IOrderDiffGenerator>();

            //assess
            var mainViewModel = new MainViewModel(commandService, logService.Object, lifetimeService.Object, orderDiffGenerator.Object);

            //assert
            Assert.IsTrue(mainViewModel.Model.SheetSelectionVisibility == Visibility.Collapsed);
        }

        [TestMethod]
        public void When_excel_file_is_set_sheet_selection_is_visible()
        {
            //arrange

            var logService = new Mock<ILogService>();
            var commandService = new CommandFactory();
            var orderDiffGenerator = new Mock<IOrderDiffGenerator>();

            var lifetimeService = new Mock<ILifetimeService>();
            lifetimeService.Setup(service => service.ExecuteInLifetime(It.IsAny<Func<IFileSelector,string>>())).Returns("fileName");
            lifetimeService.Setup(service => service.ExecuteInLifetime(It.IsAny<Func<IExcelReader, IEnumerable<string>>>())).Returns(new List<string>());
            
            var mainViewModel = new MainViewModel(commandService, logService.Object, lifetimeService.Object, orderDiffGenerator.Object);

            //assess
            mainViewModel.Model.SelectExcelFileCommand.Execute(null);
            Thread.Sleep(1000);//SUPER YUCKY WAY TO BYPASS AWAIT IN COMMAND
            //this happens because of async void command

            //assert
            Assert.IsTrue(mainViewModel.Model.SheetSelectionVisibility == Visibility.Visible);
        }
    }
}
