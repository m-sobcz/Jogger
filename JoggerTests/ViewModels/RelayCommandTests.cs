using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels.Tests
{
    [TestClass()]
    public class RelayCommandTests
    {
        [TestMethod()]
        public void SingleActionExecution()
        {
            var executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            relayCommand.Execute(new object());
            Assert.AreEqual(1, executionCounter);
        }
        [TestMethod()]
        public void DoubleActionExecution()
        {
            var executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            relayCommand.Execute(new object());
            relayCommand.Execute(new object());
            Assert.AreEqual(2, executionCounter);
        }
        [TestMethod()]
        public void CanNotExecuteWithTypedFalse()
        {
            var executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => false);
            Assert.AreEqual(false, relayCommand.CanExecute(new object()));
        }
        [TestMethod()]
        public void CanExecuteWithDefaultCanExecute()
        {
            var executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++);
            Assert.AreEqual(true, relayCommand.CanExecute(new object()));
        }
    }
}