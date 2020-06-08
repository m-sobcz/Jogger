using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ValveTypes
{
    public interface IValveType
    {
        public Dictionary<short, string> ErrorCodes { get; set; } 
        public List<Query> QueryList { get; set; }
    }
}
