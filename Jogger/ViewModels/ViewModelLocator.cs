using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class ViewModelLocator
    {
        IServiceProvider serviceProvider;
        public ViewModelLocator()
        {
            this.serviceProvider = App.ServiceProvider;
        }
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<MainWindowViewModel>();
            }
        }
        public JoggingViewModel JoggingViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<JoggingViewModel>();
            }
        }
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<SettingsViewModel>();
            }
        }
        public DiagnosticsViewModel DiagnosticsViewModel
        {
            get
            {
                return serviceProvider.GetRequiredService<DiagnosticsViewModel>();
            }
        }
    }
}
