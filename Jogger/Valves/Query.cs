using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valves
{
    public class Query
    {
        readonly List<Command> Commands = new List<Command>();
        protected int step;
        public bool isDone = false;
        public QueryType queryType;
        public Query(QueryType queryType = QueryType.singleExecution)
        {
            this.queryType = queryType;
        }
        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }
        public async Task<string> ExecuteStep(IDriver driver, ulong accessMask)
        {
            try
            {
               if (!isDone) driver.SetSendData(Commands[step].sendData, Commands[step].id, Command.dataLengthCode, accessMask);
            }
            catch (Exception e)
            {
                System.Diagnostics.Trace.WriteLine(e.Message+ e.StackTrace);
            }
            string message = await Task<string>.Run(() => driver.Send());
            step++;
            if (step >= Commands.Count)
            {
                isDone = true;
            }
            return message;
        }
        public void Restart()
        {
            step = 0;
            isDone = false;
        }
    }
    public enum QueryType
    {
        singleExecution,
        inflate,
        deflate
    }
}
