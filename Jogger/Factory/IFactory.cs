using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Factory
{
    public interface IFactory<T>
    {
        T Get();
    }
}
