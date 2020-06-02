using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Factory
{
    class ServiceProviderFactory<T> : IFactory<T>
    {
        IServiceProvider serviceProvider;
        ServiceProviderFactory()
        {
            //IServiceProvider serviceProvider
            
            //this.serviceProvider = serviceProvider;
        }

        public T Get()
        {
            return serviceProvider.GetRequiredService<T>();
        }
    }
}
