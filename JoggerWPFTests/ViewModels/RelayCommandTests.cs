using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Input;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Jogger.ViewModels.Tests
{
    [TestClass()]
    public class RelayCommandTests
    {
        [TestMethod()]
        public void Execute_Double_CounterEquals2()
        {
            //Arrange
            int executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            //Act
            relayCommand.Execute(new object());
            relayCommand.Execute(new object());
            //Assert
            Assert.AreEqual(2, executionCounter);
        }
        [TestMethod()]
        public void Execute_CanExecuteSetToFalse_AreEqual()
        {
            //Arrange
            int executionCounter = 0;
            //Act
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => false);
            //Assert
            Assert.AreEqual(false, relayCommand.CanExecute(new object()));
        }
        [TestMethod()]
        public void Execute_CanExecuteWithDefaultCanExecuteParameter_CanExecute()
        {
            //Arrange
            int executionCounter = 0;
            //Act
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++);
            //Assert
            Assert.AreEqual(true, relayCommand.CanExecute(new object()));
        }
    }
}

