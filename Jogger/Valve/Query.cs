using Jogger.Drivers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Jogger.Valve
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
        public async Task<string> ExecuteStep(IDriver driver)
        {
            string s = await Commands[step].SendDriverRequest(driver);
            step++;
            if (step >= Commands.Count)
            {
                isDone = true;
            }
            return s;
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
