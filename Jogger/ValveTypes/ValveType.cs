using Jogger.Valves;
using System;
using System.Collections.Generic;
using System.Text;

namespace Jogger.ValveTypes
{
    public class ValveType : IValveType
    {
        public Dictionary<short, string> ErrorCodes { get; set; } = new Dictionary<short, string>();
        public List<Query> QueryList { get; set; } = new List<Query>();
        public ValveType()
        {
            byte[] b = new byte[4];
            b[0] = 0x00;
            b[1] = 0x00;
            ErrorCodes.Add(BitConverter.ToInt16(b, 0), "<no error>");

            QueryList.Add(GetTesterPresentQuery());
            QueryList.Add(GetTesterPresentQuery());
            QueryList.Add(GetTesterPresentQuery());
        }

        protected Query GetTesterPresentQuery()
        {
            Query query = new Query();
            query.AddCommand(new Command(0x3c, new byte[] { 0x90, 0x0, 0x3E, 0x0 }));
            query.AddCommand(new Command(0x3d));
            return query;
        }
    }
}
