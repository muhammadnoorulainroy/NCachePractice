using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DataModel;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime;
using Alachisoft.NCache.Runtime.Dependencies;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace Dependency
{
    public class AggregateDependency
    {
        private static ICache cache;
        private static string dependencyFilePath = "C:\\Users\\noor_ul_ain\\Desktop\\Dependency file.txt";
        public static void Run()
        {
            Initialize();
            cache.Clear();
            AddItemWithAggregateDependency();
            //RemoveItem("1");
            Console.WriteLine(cache.Count);
        }

        private static void Initialize()
        {
            cache = CacheManager.GetCache("Dummy");
            Console.WriteLine("Cache has been initialized");
        }

        private static void AddItemWithAggregateDependency()
        { 
            var product = new Product { Id = 1, Name = "Coffee", Price = 200 };
            cache.Add(product.Id.ToString(), product);

            AggregateCacheDependency aggregateCacheDependency = new AggregateCacheDependency();
            aggregateCacheDependency.Add(new KeyDependency(product.Id.ToString()));
            aggregateCacheDependency.Add(new FileDependency(dependencyFilePath));


            Order order = new Order { Id = 2, Price = 200, ShippingAddress = "Diya Tech" };

            CacheItem cacheItem = new CacheItem(order); 
            cacheItem.Dependency = aggregateCacheDependency;
            cache.Add(order.Id.ToString(), cacheItem);
        }

        private static void RemoveItem(string key)
        {
            cache.Remove(key);
            Console.WriteLine("Item {0} has been removed.", key);
        }

    }
}
