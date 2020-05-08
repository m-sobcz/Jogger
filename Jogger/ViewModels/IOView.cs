using Jogger.Services;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Jogger.ViewModels
{
    class IOView : ObservedObject
    {
        IDictionary<int, DigitalState> io = new Dictionary<int, DigitalState>();

        [IndexerName("Item")]
        public DigitalState this[int index]
        {
            get
            {
                if (io.ContainsKey(index))
                {
                    return io[index];
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
            }
            set
            {
                io[index] = value;
                OnPropertyChanged($"Item[{index}]");
            }

        }
    }
}
