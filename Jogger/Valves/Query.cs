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
        static int count = 0;
        int id;
        protected int step=0;
        public bool isDone = false;
        public QueryType queryType;
        public Query(QueryType queryType = QueryType.singleExecution)
        {
            this.queryType = queryType;
            id = count++;
        }
        public void AddCommand(Command command)
        {
            Commands.Add(command);
        }
        public async Task<string> ExecuteStep(IDriver driver, int channelNumber)
        {
            try
            {
               if (!isDone) driver.SetSendData(Commands[step].sendData, Commands[step].id, Command.dataLengthCode, channelNumber);
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
