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
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            relayCommand.Execute(new object());
            relayCommand.Execute(new object());
            Assert.AreEqual(2, executionCounter);
        }
        [TestMethod()]
        public void CanNotExecuteWithTypedFalse()
        {
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => false);
            Assert.AreEqual(false, relayCommand.CanExecute(new object()));
        }
        [TestMethod()]
        public void CanExecuteWithDefaultCanExecuteParameter()
        {
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++);
            Assert.AreEqual(true, relayCommand.CanExecute(new object()));
        }
    }
}

