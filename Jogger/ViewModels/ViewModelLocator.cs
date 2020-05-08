using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ViewModels
{
    public class ViewModelLocator
    {
        public MainWindowViewModel MainWindowViewModel
        {
            get
            {
                return App.ServiceProvider.GetRequiredService<MainWindowViewModel>();
            }
        }
        public JoggingViewModel JoggingViewModel
        {
            get
            {
                return App.ServiceProvider.GetRequiredService<JoggingViewModel>();
            }
        }
        public SettingsViewModel SettingsViewModel
        {
            get
            {
                return App.ServiceProvider.GetRequiredService<SettingsViewModel>();
            }
        }
        public DiagnosticsViewModel DiagnosticsViewModel
        {
            get
            {
                return App.ServiceProvider.GetRequiredService<DiagnosticsViewModel>();
            }
        }
    }
}
