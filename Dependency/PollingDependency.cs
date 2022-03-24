using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DataModel;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Dependencies;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace Dependency
{
    public class PollingDependency
    {
        private static string connectionString = "Data Source=NOOR-UL-AIN\\SQLEXPRESS;Initial Catalog=CashAndCarry;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User ID=DIYATECH\\noor_ul_ain";
        private static ICache cache;
        public static void Run()
        {
            Initialize();
            cache.Clear();
            AddDataWithPollingDependency();
            
        }
        private static void Initialize()
        {
            cache = CacheManager.GetCache("PRCache");
            Console.WriteLine("Cache has been initialized!!!!\n");
        }

        private static void AddDataWithPollingDependency()
        {
            for(int i = 0; i < 9; i++)
            {
                string key = i + ":dbo.Product";
                CacheDependency dependency = DBDependencyFactory.CreateSqlCacheDependency(connectionString, key);
                CacheItem item = new CacheItem(i);
                item.Dependency = dependency;

                cache.Add(key, item);
            }

            Console.WriteLine("Items added in cache with Polling Dependency");
        }

        

    }
}
