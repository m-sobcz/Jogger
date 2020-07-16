using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Jogger.IO;
using Jogger.Drivers;
using Jogger.Valves;
using Jogger.Models;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System.Threading.Tasks.Dataflow;

namespace Jogger.Services.Tests
{
    [TestClass()]
    public class TesterServiceTests
    {
        public TesterService testerService;
        Mock<IDigitalIO> digitalIOMock;
        Mock<IDriver> driverMock;
        Mock<IValveManager> valveManagerMock;
        ConfigurationSettings configurationSettings;
        TestSettings testSettings = new TestSettings();
        string valveType = "";
        [TestInitialize]
        public void TestInitializeAttribute()
        {
            digitalIOMock = new Mock<IDigitalIO>();
            driverMock = new Mock<IDriver>();
            valveManagerMock = new Mock<IValveManager>();
            configurationSettings = new ConfigurationSettings();
            testerService = new TesterService(digitalIOMock.Object, driverMock.Object, valveManagerMock.Object);
        }
        [TestMethod()]
        [DynamicData(nameof(InitializeActionStatusProgramState), DynamicDataSourceType.Method)]
        public void Initialize_ProgramStateInitializedOrErrorBasedOnActionStatuses
            (ActionStatus digitalIOActionStatus, ActionStatus driverActionStatus, ActionStatus valveManagerActionStatus, ProgramState expectedState)
        {
            //Arrange
            digitalIOMock.Setup(x => x.Initialize()).Returns(digitalIOActionStatus);
            driverMock.Setup(x => x.Initialize(4)).Returns(driverActionStatus);
            valveManagerMock.Setup(x => x.Initialize(4)).Returns(valveManagerActionStatus);
            //Act
            testerService.Initialize(4);
            //Assert
            Assert.AreEqual(expectedState, testerService.State);
        }
        public static IEnumerable<object[]> InitializeActionStatusProgramState()
        {
            yield return new object[] { ActionStatus.OK, ActionStatus.OK, ActionStatus.OK, ProgramState.Initialized };
            yield return new object[] { ActionStatus.Error, ActionStatus.OK, ActionStatus.OK, ProgramState.Error };
            yield return new object[] { ActionStatus.OK, ActionStatus.OK, ActionStatus.Error, ProgramState.Error };
        }
        [TestMethod()]
        [DynamicData(nameof(InitializeActionStatusReturnActionStatus), DynamicDataSourceType.Method)]
        public void Initialize_ReturnsCorrectActionStatus
            (ActionStatus digitalIOActionStatus, ActionStatus driverActionStatus, ActionStatus valveManagerActionStatus, ActionStatus exepctedStatus)
        {
            //Arrange
            digitalIOMock.Setup(x => x.Initialize()).Returns(digitalIOActionStatus);
            driverMock.Setup(x => x.Initialize(4)).Returns(driverActionStatus);
            valveManagerMock.Setup(x => x.Initialize(4)).Returns(valveManagerActionStatus);
            //Act
            ActionStatus returnStatus = testerService.Initialize(4);
            //Assert
            Assert.AreEqual(exepctedStatus, returnStatus);
        }
        public static IEnumerable<object[]> InitializeActionStatusReturnActionStatus()
        {
            yield return new object[] { ActionStatus.OK, ActionStatus.OK, ActionStatus.OK, ActionStatus.OK };
            yield return new object[] { ActionStatus.Error, ActionStatus.OK, ActionStatus.OK, ActionStatus.Error };
            yield return new object[] { ActionStatus.OK, ActionStatus.OK, ActionStatus.Error, ActionStatus.Error };
        }
        [TestMethod()]
        [DynamicData(nameof(StartActionStatusProgramState), DynamicDataSourceType.Method)]
        public void Start_ProgramStateStartedOrIdleBasedOnValveManagerStart(ActionStatus actionStatus, ProgramState expectedState)
        {
            //Arrange
            valveManagerMock.Setup(x => x.Start(testSettings, valveType)).Returns(actionStatus);
            //Act
            testerService.Start(testSettings, valveType);
            //Assert
            Assert.AreEqual(expectedState, testerService.State);
        }
        public static IEnumerable<object[]> StartActionStatusProgramState()
        {
            yield return new object[] { ActionStatus.OK, ProgramState.Started };
            yield return new object[] { ActionStatus.Error, ProgramState.Idle };
        }
        [TestMethod()]
        [DynamicData(nameof(StartActionStatusReturnsActionStatus), DynamicDataSourceType.Method)]
        public void Start_RetursActionStateEqualToValveMangerState(ActionStatus actionStatus)
        {
            //Arrange
            valveManagerMock.Setup(x => x.Start(testSettings, valveType)).Returns(actionStatus);
            //Act
            ActionStatus returnStatus = testerService.Start(testSettings, valveType);
            //Assert
            Assert.AreEqual(actionStatus, returnStatus);
        }
        public static IEnumerable<object[]> StartActionStatusReturnsActionStatus()
        {
            yield return new object[] { ActionStatus.OK };
            yield return new object[] { ActionStatus.Error };
        }
        [TestMethod()]
        public void Stop_SetsStateStopped()
        {
            //Act
            testerService.Stop();
            //Assert
            Assert.AreEqual(ProgramState.Stopping, testerService.State);
        }
        [TestMethod()]
        public void Stop_ReturnsActionStatusOk()
        {
            //Act
            ActionStatus returnStatus = testerService.Stop();
            //Assert
            Assert.AreEqual(ActionStatus.OK, returnStatus);
        }
        [TestMethod()]
        [DynamicData(nameof(TestingFinishedProgramStates), DynamicDataSourceType.Method)]
        public void ValveManagerTestingFinishedFiredChangesProgramState(ProgramState programStateInitial, ProgramState programStateAfterEvent)
        {
            //Arrange
            testerService.State = programStateInitial;
            //Act
            valveManagerMock.Raise(m => m.TestingFinished += null); 
            //Assert
            Assert.AreEqual(programStateAfterEvent, testerService.State);
        }
        public static IEnumerable<object[]> TestingFinishedProgramStates()
        {
            yield return new object[] { ProgramState.Started, ProgramState.Done };
            yield return new object[] { ProgramState.Stopping, ProgramState.Idle };
            yield return new object[] { ProgramState.Error, ProgramState.Error };
        }
        [TestMethod()]
        public void State_SetterFiresEvent()
        {
            //Arrange
            int eventFiredCounter = 0;
            testerService.ProgramStateChanged += (ProgramState programState) => eventFiredCounter++;
            //Act
            testerService.State = ProgramState.Done;
            testerService.State = ProgramState.Error;
            testerService.State = ProgramState.Idle;
            //Assert
            Assert.AreEqual(3, eventFiredCounter);
        }

    }
}