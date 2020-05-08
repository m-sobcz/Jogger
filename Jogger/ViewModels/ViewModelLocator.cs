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
    }
}
