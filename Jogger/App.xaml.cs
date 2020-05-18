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
            ServiceCollection serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();

            ServiceProvider.GetRequiredService<MainWindowViewModel>().showInfo.ShowInformation += ShowInfo_ShowInformation;
            ServiceProvider.GetRequiredService<JoggingViewModel>().showInfo.ShowInformation += ShowInfo_ShowInformation;
            ServiceProvider.GetRequiredService<SettingsViewModel>().showInfo.ShowInformation += ShowInfo_ShowInformation;
            ServiceProvider.GetRequiredService<DiagnosticsViewModel>().showInfo.ShowInformation += ShowInfo_ShowInformation;

            ServiceProvider.GetRequiredService<StartWindow>().Show();
        }

        private void ShowInfo_ShowInformation(object sender, string text, string caption)
        {
            MessageBox.Show(text, caption);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddSingleton<ITesterService, TesterService>();
            //Models
            services.AddSingleton<TestSettings>();
            services.AddSingleton<Models.ConfigurationSettings>();
            //ViewModels
            services.AddSingleton<MainWindowViewModel>();
            services.AddSingleton<JoggingViewModel>();
            services.AddSingleton<SettingsViewModel>();
            services.AddSingleton<DiagnosticsViewModel>();
            //Views
            services.AddTransient<StartWindow>();
            //Logic
            services.AddScoped<IValveManager, ValveManager>();
            services.AddScoped<IDigitalIO, Advantech>();
            services.AddScoped<IDriver, Driver>();
        }

        private void Application_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            MessageBox.Show($"Wystąpił nieobsłużony wyjątek: \n {e.Exception.Message}", "Nieobsłużony wyjątek", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
