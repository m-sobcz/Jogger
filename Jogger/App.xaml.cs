using Jogger.Drivers;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Jogger.Models;
using Jogger.Services;
using Jogger.ViewModels;
using Jogger.Communication;
using Jogger.IO;
using System.Diagnostics;
using Jogger.Views;
using Jogger.Valves;

namespace Jogger
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public IConfiguration Configuration { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            Configuration = builder.Build();
            // Create a service collection and configure our dependencies
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            // Build the our IServiceProvider and set our static reference to it
            ServiceProvider = serviceCollection.BuildServiceProvider();
            ServiceProvider.GetRequiredService<TestSettings>();
            ServiceProvider.GetRequiredService<Services.ConfigurationSettings>();
            StartWindow startWindow = ServiceProvider.GetRequiredService<StartWindow>();
            ServiceProvider.GetRequiredService<MainWindowViewModel>().showInfo.ShowInformation += ShowInfo_ShowInformation;
            //ServiceProvider.GetRequiredService<MainWindowViewModel>();
            //startWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            Trace.WriteLine(MainWindow.DataContext);
            startWindow.Show();
        }

        private void ShowInfo_ShowInformation(object sender, string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddScoped<IDriver, DriverStub>();
            services.AddScoped<Valve>();
            services.AddSingleton<ITesterService, TesterService>();
            services.AddSingleton<TestSettings>();
            services.AddSingleton<Services.ConfigurationSettings>();
            //ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<JoggingViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<DiagnosticsViewModel>();
            //Views
            services.AddTransient<StartWindow>();
            //Stubs
            services.AddScoped<ICommunication, CommunicationStub>();
            services.AddScoped<IDigitalIO, DigitalIOStub>();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Wystąpił nieobsłużony wyjątek: \n {e.Exception.Message}", "Nieobsłużony wyjątek", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
