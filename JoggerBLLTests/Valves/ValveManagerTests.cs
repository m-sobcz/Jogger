using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;
using Jogger.Models;
using Moq;
using Castle.DynamicProxy.Generators.Emitters.SimpleAST;
using Jogger.Drivers;
using Jogger.Services;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;

namespace Jogger.Valves.Tests
{
    [TestClass()]
    public class ValveManagerTests
    {
        ValveManager valveManager;
        TestSettings testSettings;
        Mock<Func<IValve>> getValveMock;
        [TestInitialize]
        public void TestInitializeAttribute()
        {
            testSettings = new TestSettings();
            getValveMock = new Mock<Func<IValve>>();
            getValveMock.Setup(x => x.Invoke()).Returns(new Mock<IValve>().Object);
            valveManager = new ValveManager(testSettings, getValveMock.Object);
        }
        [DataTestMethod]
        [DataRow(1, 1)]
        [DataRow(4, 4)]
        [DataRow(0, 0)]
        [DataRow(-1, 0)]
        public void Initialize_CreateAsManyValvesAsChannelCount(int channelsCount, int expectedValvesCount)
        {
            //Act
            valveManager.Initialize(channelsCount);
            //Assert
            Assert.AreEqual(expectedValvesCount, valveManager.valves.Count);
        }
        [TestMethod()]
        [DynamicData(nameof(StartData_StringActionStatus), DynamicDataSourceType.Method)]
        public void Start_returnsActionStatusOkIfValveTypeExists(string valveTypeTxt, ActionStatus expectedActionStatus)
        {
            //Arrange
            valveManager.Initialize(4);
            //Act
            ActionStatus returnActionStatus = valveManager.Start(testSettings, valveTypeTxt);
            //Assert
            Assert.AreEqual(expectedActionStatus, returnActionStatus);
        }
        public static IEnumerable<object[]> StartData_StringActionStatus()
        {
            yield return new object[] { "", ActionStatus.OK };
            yield return new object[] { "2Up", ActionStatus.OK };
            yield return new object[] { "3_5Up", ActionStatus.OK };
            yield return new object[] { "Unknown", ActionStatus.Error };
        }
        [TestMethod()]
        public void Stop_SetsIsStopRequested()
        {
            //Arrange           
            for (int i = 0; i < 4; i++)
            {
                Mock<IValve> valveMock = new Mock<IValve>().SetupProperty(m => m.IsStopRequested);
                valveManager.valves.Add(valveMock.Object);
            }
            bool valveWithoutStopRequestExist = false;
            //Act
            valveManager.Stop();
            foreach (IValve valve in valveManager.valves)
            {
                if (valve.IsStopRequested == false) valveWithoutStopRequestExist = true;
            }
            //Assert
            Assert.IsFalse(valveWithoutStopRequestExist);
        }
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void Send_ExecutesActualValveStep(int actualProcessedChannel)
        {
            //Arrange
            Mock<IValve> processedValveMock = GetProcessedValveMockAndSetupValveManager(actualProcessedChannel);
            //Act
            Task<bool> result = valveManager.Send();
            //Assert
            processedValveMock.Verify(m => m.ExecuteStep(), Times.Once);
        }
        [DataTestMethod]
        [DataRow(0)]
        [DataRow(3)]
        public void Received_ReceivesActualValveStep(int actualProcessedChannel)
        {
            //Arrange
            Mock<IValve> processedValveMock = GetProcessedValveMockAndSetupValveManager(actualProcessedChannel);
            //Act
            Task<bool> result = valveManager.Receive();
            //Assert
            processedValveMock.Verify(m => m.Receive(), Times.Once);
        }
        Mock<IValve> GetProcessedValveMockAndSetupValveManager(int actualProcessedChannel)
        {
            Mock<IValve> processedValveMock = new Mock<IValve>();
            for (int i = 0; i < 4; i++)
            {
                Mock<IValve> valveMock = new Mock<IValve>();
                valveMock.Setup((x) => x.ExecuteStep()).Returns(Task.FromResult("Executed"));
                valveMock.Setup((x) => x.Receive()).Returns(Task.FromResult("Received"));
                if (i == actualProcessedChannel)
                {
                    processedValveMock = valveMock;
                }
                valveManager.valves.Add(valveMock.Object);
            }
            valveManager.ActualProcessedValve = actualProcessedChannel;
            return processedValveMock;
        }
        [TestMethod()]
        [DynamicData(nameof(SetNextProcessedValveData), DynamicDataSourceType.Method)]
        public void SetNextProcessedValve_SetsProperActualProcessedValve(int startProcessedValve, int endProcessedValve, bool[] isStarted)
        {
            //Arrange
            for (int i = 0; i < 4; i++)
            {
                Mock<IValve> valveMock = new Mock<IValve>();
                valveMock.SetupGet(m => m.IsStarted).Returns(isStarted[i]);
                valveManager.valves.Add(valveMock.Object);
            }
            valveManager.ActualProcessedValve = startProcessedValve;
            //Act
            valveManager.SetNextProcessedValve();
            //Assert
            Assert.AreEqual(endProcessedValve, valveManager.ActualProcessedValve);
        }
        public static IEnumerable<object[]> SetNextProcessedValveData()
        {
            yield return new object[] { 2, 3, new bool[] { true, true, true, true } };
            yield return new object[] { 1, 1, new bool[] { false, false, false, false } };
            yield return new object[] { 0, 3, new bool[] { true, false, false, true } };
        }
    }
}