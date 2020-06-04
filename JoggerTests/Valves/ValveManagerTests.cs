using Microsoft.VisualStudio.TestTools.UnitTesting;
using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Jogger.IO;
using Jogger.Models;
using Jogger.Drivers;
using Jogger.Services;
using Microsoft.VisualStudio.TestPlatform.Utilities;
using System.Reflection;

namespace Jogger.Valves.Tests
{
    [TestClass()]
    public class ValveManagerTests
    {
        ValveManager valveManager; 
        public static IServiceProvider ServiceProvider { get; private set; }
        public IConfiguration Configuration { get; private set; }

        [TestInitialize]
        public void TestInitializeAttribute()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            valveManager = ServiceProvider.GetRequiredService<ValveManager>();
        }
        private void ConfigureServices(ServiceCollection serviceCollection)
        {
            //services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            //Logic - Core
            serviceCollection.AddSingleton<ValveManager>();
            serviceCollection.AddSingleton<TestSettings>();
            serviceCollection.AddSingleton<IDigitalIO, Advantech>();
            serviceCollection.AddSingleton<IDriver, DriverStub>();
            //Logic - Other 
            serviceCollection.AddTransient<IValve, Valve>();
            serviceCollection.AddSingleton<Func<IValve>>(x => () => x.GetRequiredService<IValve>());
        }
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        public void Initialize_CreatesAsManyValvesAsNumberOfChannels(int numberOfChannels)
        {
            valveManager.Initialize(numberOfChannels);
            Assert.AreEqual(numberOfChannels, valveManager.valves.Count);
        }
        [DataTestMethod]
        [DataRow(1)]
        [DataRow(4)]
        public void Initialize_ReturnsActionStatusOk(int numberOfChannels)
        {
            ActionStatus actionStatus =valveManager.Initialize(numberOfChannels);
            Assert.AreEqual(ActionStatus.OK, actionStatus);
        }
       
    }
}