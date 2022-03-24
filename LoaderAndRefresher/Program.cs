using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LoaderRefresher;


namespace LoaderAndRefresher
{
    internal class Program
    {
        static void Main(string[] args)
        {

            /*LoaderRefresher.LoaderRefresher lr = new LoaderRefresher.LoaderRefresher();

            lr.Init(null, "");
            lr.LoadDatasetOnStartup("products");*/

            UsageOfCacheLoaderAndRefresher.Run();
            Console.ReadKey();
        }
    }
}
