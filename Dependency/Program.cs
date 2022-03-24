using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Dependency
{
    internal class Program
    {
        static void Main(string[] args)
        {

            //KeyBasedDependency.Run();
            //FileBasedDependency.Run();
            //DatabaseDependency.Run();
            //PollingDependency.Run();
            AggregateDependency.Run();
            Console.ReadKey();
        }
    }
}
