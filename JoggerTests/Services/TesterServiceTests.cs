using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Jogger.IO;
using Jogger.Communication;
using System.Threading.Tasks;

namespace Jogger.Services.Tests
{
    [TestClass()]
    public class TesterServiceTests
    {
        [TestMethod()]
        public void Initialize_InitializationSuccess_SetsStateInitialized()
        {     
            TesterService testerService = new TesterService(new CommunicationStub(), new DigitalIOStub(ActionStatus.OK, "OK", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }));
            ActionStatus actionStatus = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Initialized, testerService.State);
        }
        [TestMethod()]
        public void Initialize_InitializationSuccess_ReturnsActionStatusOk()
        {
            TesterService testerService = new TesterService(new CommunicationStub(), new DigitalIOStub(ActionStatus.OK, "OK", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 }));
            ActionStatus status= testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ActionStatus.OK, status);
        }
        [TestMethod()]
        public void Initialize_InitializationFailed_SetsStateError()
        {
            TesterService testerService = new TesterService(new CommunicationStub(), new DigitalIOStub(ActionStatus.Error, "Error", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 })) {};
            ActionStatus status=testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Initialize_InitializationFailed_ReturnsActionStatusError()
        {
            TesterService testerService = new TesterService(new CommunicationStub(), new DigitalIOStub(ActionStatus.Error, "Error", new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 })) { };
            ActionStatus status = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ActionStatus.Error,status);
        }
    }
}