using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;
using Jogger.IO;
using Jogger.Models;
using Jogger.Drivers;
using Jogger.Services;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Reflection;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Jogger.Valves.Tests
{
    //[TestClass()]
    //public class ValveManagerTests
    //{
    //    ValveManager valveManager;
    //    public static IServiceProvider ServiceProvider { get; private set; }
    //    public IConfiguration Configuration { get; private set; }
    //    [ClassInitialize]
    //    public static void ClassInitialize(TestContext testContext)
    //    {
    //        ServiceCollection serviceCollection = new ServiceCollection();
    //        serviceCollection.AddTransient<IValveManager, ValveManager>();
    //        serviceCollection.AddSingleton<TestSettings>();
    //        serviceCollection.AddSingleton<IDriver, DriverStub>();
    //        serviceCollection.AddTransient<IValve, ValveStub>();
    //        serviceCollection.AddSingleton<Func<IValve>>(x => () => x.GetRequiredService<IValve>());
    //        ServiceProvider = serviceCollection.BuildServiceProvider();
    //    }
    //    [TestInitialize]
    //    public void TestInitialize()
    //    {
    //        valveManager = (ValveManager)ServiceProvider.GetRequiredService<IValveManager>();
    //    }
    //    [DataTestMethod]
    //    [DataRow(1)]
    //    [DataRow(4)]
    //    public void Initialize_CreatesAsManyValvesAsNumberOfChannels(int numberOfChannels)
    //    {
    //        valveManager.Initialize(numberOfChannels);
    //        Assert.AreEqual(numberOfChannels, valveManager.valves.Count);
    //    }
    //    [DataTestMethod]
    //    [DataRow(1)]
    //    [DataRow(4)]
    //    public void Initialize_ReturnsActionStatusOk(int numberOfChannels)
    //    {
    //        ActionStatus actionStatus = valveManager.Initialize(numberOfChannels);
    //        Assert.AreEqual(ActionStatus.OK, actionStatus);
    //    }

    //    [DataTestMethod]
    //    [DataRow(false, true)]
    //    [DataRow(true, false)]
    //    public void SetValveSensorsState_TargetValveIsInflatedAndDeflatedAccoringToArguments(bool isInflated, bool isDeflated)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        int valveNumber = 0;
    //        valveManager.SetValveSensorsState(valveNumber, isInflated, isDeflated);
    //        Assert.AreEqual(isInflated, valveManager.valves[valveNumber].IsInflated);
    //        Assert.AreEqual(isDeflated, valveManager.valves[valveNumber].IsDeflated);
    //    }

    //    [DataTestMethod]
    //    [DataRow(4, 3, true)]
    //    [DataRow(4, 4, false)]
    //    public void SetValveSensorsState_ReturnsTrueIfAbleToSetElement(int numberOfValves, int valveNumber, bool expectedReturn)
    //    {
    //        for (int i = 0; i < numberOfValves; i++)
    //        {
    //            valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        }
    //        bool result = valveManager.SetValveSensorsState(valveNumber, true, false);
    //        Assert.AreEqual(expectedReturn, result);
    //    }
    //    [TestMethod()]
    //    public void Start_ValveTypeIsNull_ReturnsActionStatusError()
    //    {
    //        ActionStatus actionStatus = valveManager.Start(ServiceProvider.GetRequiredService<TestSettings>(), null);
    //        Assert.AreEqual(ActionStatus.Error, actionStatus);
    //    }
    //    [DataTestMethod]
    //    [DataRow("")]
    //    [DataRow("2Up")]
    //    [DataRow("3_5Up")]
    //    public void Start_ValveTypeIsSupported_ReturnsActionStatusOk(string valveTypeTxt)
    //    {
    //        ActionStatus actionStatus = valveManager.Start(ServiceProvider.GetRequiredService<TestSettings>(), valveTypeTxt);
    //        Assert.AreEqual(ActionStatus.OK, actionStatus);
    //    }
    //    [TestMethod()]
    //    public void Start_ValveTypeNotSupported_ReturnsActionStatusError()
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        ActionStatus actionStatus = valveManager.Start(ServiceProvider.GetRequiredService<TestSettings>(), "Test");
    //        Assert.AreEqual(ActionStatus.Error, actionStatus);
    //    }
    //    [TestMethod()]
    //    public void Stop_ReturnsActionStatusOk()
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        ActionStatus actionStatus = valveManager.Stop();
    //        Assert.AreEqual(ActionStatus.OK, actionStatus);
    //    }
    //    [TestMethod()]
    //    public void Stop_SetsIsStopRequestedOnAllValves()
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.Stop();
    //        foreach (IValve valve in valveManager.valves)
    //        {
    //            Assert.AreEqual(true, valve.IsStopRequested);
    //        }
    //    }
    //    [TestMethod]
    //    public void Send_ExecuteStepDone()
    //    {
    //        ValveStub valve = (ValveStub)ServiceProvider.GetRequiredService<IValve>();
    //        valveManager.valves.Add(valve);
    //        Task<bool> task = valveManager.Send();
    //        Assert.IsTrue(valve.executeStepDone);
    //    }
    //    [DataTestMethod]
    //    [DataRow(false)]
    //    [DataRow(true)]
    //    public void Send_TriggerCommunicationLogChangedIfLogOutDataSelected(bool isLogOutDataSelected)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        TestSettings testSettings = ServiceProvider.GetRequiredService<TestSettings>();
    //        testSettings.IsLogOutDataSelected = isLogOutDataSelected;
    //        bool communicationLogChangedTriggered = false;
    //        valveManager.CommunicationLogChanged += (object sender, string log) => communicationLogChangedTriggered = true;
    //        Task<bool> task = valveManager.Send();
    //        task.Wait();
    //        Assert.AreEqual(isLogOutDataSelected, communicationLogChangedTriggered);
    //    }
    //    [TestMethod]
    //    public void Receive_ValveReceiveExecuted()
    //    {
    //        ValveStub valve = (ValveStub)ServiceProvider.GetRequiredService<IValve>();
    //        valveManager.valves.Add(valve);
    //        Task<bool> task = valveManager.Receive();
    //        Assert.IsTrue(valve.receiveDone);
    //    }
    //    [DataTestMethod]
    //    [DataRow(false)]
    //    [DataRow(true)]
    //    public void Receive_TriggerCommunicationLogChangedIfLogInDataSelected(bool isLogInDataSelected)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        TestSettings testSettings = ServiceProvider.GetRequiredService<TestSettings>();
    //        testSettings.IsLogInDataSelected = isLogInDataSelected;
    //        bool communicationLogChangedTriggered = false;
    //        valveManager.CommunicationLogChanged += (object sender, string log) => communicationLogChangedTriggered = true;
    //        Task<bool> task = valveManager.Receive();
    //        task.Wait();
    //        Assert.AreEqual(isLogInDataSelected, communicationLogChangedTriggered);
    //    }
    //    [DataTestMethod]
    //    [DataRow(0, 0)]
    //    [DataRow(1, 1)]
    //    [DataRow(2, 0)]
    //    [DataRow(3, 1)]
    //    [DataRow(4, 0)]
    //    public void SetNextProcessedChannel_TwoValvesAndBothStarted_ActualProceseedValveSwitchesBetweenValves(int numberOfExecutions, int expectedActualProcessedValve)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves[0].IsStarted = true;
    //        valveManager.valves[1].IsStarted = true;
    //        for (int i = 0; i < numberOfExecutions; i++)
    //        {
    //            valveManager.SetNextProcessedValve();
    //        }
    //        Assert.AreEqual(expectedActualProcessedValve, valveManager.ActualProcessedValve);
    //    }
    //    [DataTestMethod]
    //    [DataRow(0)]
    //    [DataRow(1)]
    //    [DataRow(2)]
    //    public void SetNextProcessedChannel_OneValveStarted_OnlyStartedValveIsSetAsActualProcessedValve(int numberOfStartedValve)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves[numberOfStartedValve].IsStarted = true;
    //        valveManager.SetNextProcessedValve();
    //        Assert.AreEqual(numberOfStartedValve, valveManager.ActualProcessedValve);
    //    }
    //    [DataTestMethod]
    //    [DataRow(0)]
    //    [DataRow(1)]
    //    [DataRow(2)]
    //    public void SetNextProcessedChannel_NoValvesStarted_ActualProcessedValveDoesntChange(int numberOfStartedValve)
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        int previousValve = valveManager.ActualProcessedValve;
    //        valveManager.SetNextProcessedValve();
    //        Assert.AreEqual(previousValve, valveManager.ActualProcessedValve);
    //    }
    //    [TestMethod]
    //    public void SetNextProcessedChannel_NoValvesStarted_TestingFinishedEventTriggered()
    //    {
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        valveManager.valves.Add(ServiceProvider.GetRequiredService<IValve>());
    //        bool testingFinishedTriggered = false;
    //        valveManager.TestingFinished += (object sender, EventArgs e) => testingFinishedTriggered = true;
    //        valveManager.SetNextProcessedValve();
    //        Assert.IsTrue(testingFinishedTriggered);
    //    }


    //}
}