using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.Models
{
    public class ValveModel
    {
        public string Code { get; set; } = "";
        public string Name { get; set; } = "";
        public ValveModel(string code, string name)
        {
            this.Code = code;
            this.Name = name;
        }
    }
}
