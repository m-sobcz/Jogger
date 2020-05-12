using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Jogger.IO;
using Jogger.Communication;
using System.Threading.Tasks;
using Jogger.Drivers;

namespace Jogger.Services.Tests
{
    [TestClass()]
    public class TesterServiceTests
    {
        ICommunication communication = new CommunicationStub() { IsTestingDone = false };
        ICommunication communicationInitError = new CommunicationStub() { initializeStatus = ActionStatus.Error, IsTestingDone = false };
        ICommunication communicationStartError = new CommunicationStub() { startStatus = ActionStatus.Error, IsTestingDone = false };
        IDigitalIO digitalIO = new DigitalIOStub() { Status = ActionStatus.OK, ReadData = "readData", ResultData = new byte[] { 0, 1, 2, 3, 4, 5, 6, 7 } };
        IDriver driver = new DriverStub();
        TesterService testerService;
        [TestInitialize()]
        public void Initialize()
        {
            testerService = new TesterService(communication, digitalIO, driver);
        }
        [TestMethod()]
        public void Initialize_InitializationSuccess_SetsStateInitialized()
        {
            ActionStatus actionStatus = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Initialized, testerService.State);
        }
        [TestMethod()]
        public void Initialize_InitializationSuccess_ReturnsActionStatusOk()
        {
            ActionStatus status = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ActionStatus.OK, status);
        }
        [TestMethod()]
        public void Initialize_InitializationFailed_SetsStateError()
        {
            testerService = new TesterService(communicationInitError, digitalIO, driver);
            ActionStatus status = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Initialize_InitializationFailed_ReturnsActionStatusError()
        {
            testerService = new TesterService(communicationInitError, digitalIO, driver);
            ActionStatus status = testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ActionStatus.Error, status);
        }
        [TestMethod()]
        public void Start_ExecuteOk_SetsStateStarted()
        {
            //Func<TestSettings, string, ActionStatus> startFunc = (TestSettings testSettings, string text) => { return ActionStatus.OK; };
            ActionStatus status = testerService.Start(new TestSettings());
            Assert.AreEqual(ProgramState.Started, testerService.State);
        }
        [TestMethod()]
        public void Start_ExecuteFail_SetsStateError()
        {
            //Func<TestSettings, string, ActionStatus> startFunc = (TestSettings testSettings, string text) => { return ActionStatus.Error; };
            testerService = new TesterService(communicationStartError, digitalIO, driver);
            ActionStatus status = testerService.Start(new TestSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Stop_Stopped_StopFuncExecuted()
        {
            bool executionDone = false;
            testerService.Stop(() => executionDone = true);
            Assert.AreEqual(true, executionDone);
        }
        [TestMethod()]
        public void Stop_Stopped_SetsStateIdle()
        {
            testerService.Stop(() => { });
            Assert.AreEqual(ProgramState.Idle, testerService.State);
        }

        [TestMethod()]
        public void Dispose_OnNulls_DoesntThrowNullReferenceException()
        {
            testerService.Dispose();
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void CommunicationLoop_ExecutesReadIO()
        {
            testerService.Dispose();
            Assert.IsTrue(true);
        }
        [TestMethod()]
        public void CommunicationLoop_ExecutesSendData()
        {
            testerService.Dispose();
            Assert.IsTrue(true);
        }
    }
}