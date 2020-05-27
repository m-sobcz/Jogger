using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Jogger.IO;
using Jogger.Drivers;
using Jogger.Valves;
using Jogger.Models;
using Microsoft.VisualStudio.TestPlatform.Utilities;

namespace Jogger.Services.Tests
{
    [TestClass()]
    public class TesterServiceTests
    {
        public TesterService testerService;
        public DigitalIOStub digitalIOStub = new DigitalIOStub();
        public DriverStub driverStub = new DriverStub();
        public ValveManagerStub valveManagerStub = new ValveManagerStub();
        TestSettings testSettings = new TestSettings();
        [TestInitialize]
        public void TestInitializeAttribute()
        {
            IDigitalIO digitalIO = digitalIOStub;
            IDriver driver = driverStub;
            IValveManager valveManager = valveManagerStub;
            testerService = new TesterService(digitalIO, driver, valveManager);
        }
        [TestMethod()]
        public void Initialize_AllInitialziationStatusesOk_StateIsInitialized()
        {
            testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Initialized, testerService.State);
        }
        [TestMethod()]
        public void Initialize_DriverInitializationStatusIsError_StateIsError()
        {
            driverStub.initializationStatus = ActionStatus.Error;
            testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Initialize_DigitalIOInitializationStatusIsError_StateIsError()
        {
            digitalIOStub.initializationStatus = ActionStatus.Error;
            testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Initialize_ValveManagerInitializationStatusIsError_StateIsError()
        {
            valveManagerStub.initializationStatus = ActionStatus.Error;
            testerService.Initialize(new ConfigurationSettings());
            Assert.AreEqual(ProgramState.Error, testerService.State);
        }
        [TestMethod()]
        public void Start_valveManagerStartReturnsOk_StateIsStarted()
        {
            testerService.Start(testSettings);
            Assert.AreEqual(ProgramState.Started, testerService.State);
        }

        [TestMethod()]
        public void Start_valveManagerStartReturnsError_StateIsIdle()
        {
            valveManagerStub.startStatus = ActionStatus.Error;
            testerService.Start(testSettings);
            Assert.AreEqual(ProgramState.Idle, testerService.State);
        }
        [TestMethod()]
        public void Stop_Execution_SetsStateStopping()
        {
            testerService.Stop();
            Assert.AreEqual(ProgramState.Stopping, testerService.State);
        }
        [TestMethod()]
        public void TestingFinished_StateIsStarted_SetsStateDone()
        {
            testerService.State = ProgramState.Started;
            valveManagerStub.OnTestingFinished();
            Assert.AreEqual(ProgramState.Done, testerService.State);
        }
        [TestMethod()]
        [DynamicData(nameof(GetTestingFinishedData), DynamicDataSourceType.Method)]
        public void TestingFinished_IfStartedOrStopping_ChangeToDoneOrIdleElseDontChange(ProgramState initialState, ProgramState finalState)
        {
            testerService.State = initialState;
            valveManagerStub.OnTestingFinished();
            Assert.AreEqual(finalState, testerService.State);
        }
        [TestMethod()]
        public void Dispose_DriverAndDigitalIODisposed()
        {
            testerService.Dispose();
            Assert.AreEqual(true,driverStub.isDisposed&digitalIOStub.isDisposed);
        }
        public static IEnumerable<object[]> GetTestingFinishedData()
        {
            yield return new object[] { ProgramState.Started, ProgramState.Done };
            yield return new object[] { ProgramState.Stopping, ProgramState.Idle };
            yield return new object[] { ProgramState.Error, ProgramState.Error };
            yield return new object[] { ProgramState.Initialized, ProgramState.Initialized };
        }
    }
}