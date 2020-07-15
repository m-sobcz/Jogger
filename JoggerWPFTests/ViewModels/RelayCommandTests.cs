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
        public void Execute_Once_CounterEquals1()
        {
            var executionCounter = 0;
            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            relayCommand.Execute(new object());
            Assert.AreEqual(1, executionCounter);
        }
        [TestMethod()]
        public void Execute_Double_CounterEquals2()
        {
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => true);
            relayCommand.Execute(new object());
            relayCommand.Execute(new object());
            Assert.AreEqual(2, executionCounter);
        }
        [TestMethod()]
        public void Execute_CanExecuteSetToFalse_AreEqual()
        {
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++, o => false);
            Assert.AreEqual(false, relayCommand.CanExecute(new object()));
        }
        [TestMethod()]
        public void Execute_CanExecuteWithDefaultCanExecuteParameter_CanExecute()
        {
            int executionCounter = 0;

            RelayCommand relayCommand = new RelayCommand((o) => executionCounter++);
            Assert.AreEqual(true, relayCommand.CanExecute(new object()));
        }
    }
}

