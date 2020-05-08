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
            MainWindow mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            ServiceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.DataContext = ServiceProvider.GetRequiredService<MainWindowViewModel>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient(typeof(MainWindow));
            services.Configure<AppSettings>(Configuration.GetSection(nameof(AppSettings)));
            services.AddScoped<ISampleService, SampleService>();
            services.AddScoped<IDriver, Driver>();
            services.AddSingleton<ITesterService, TesterService>();
            services.AddScoped<MainWindowViewModel>();
            services.AddScoped<JoggingViewModel>();
            services.AddScoped<SettingsViewModel>();
            services.AddScoped<DiagnosticsViewModel>();
            //stubs
            services.AddScoped<ICommunication, CommunicationStub>();
            services.AddScoped<IDigitalIO, DigitalIOStub>();          
        }
    }
}
