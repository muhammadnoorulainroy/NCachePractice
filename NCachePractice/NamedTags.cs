using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Runtime.Caching;
using System.Configuration;
using DataModel;

namespace NCachePractice
{
    /// <summary>
    /// Class that demonstrates the usage of NamedTags in NCache.
    /// </summary>
    public class NamedTags
    {
        private static ICache _cache;

        /// <summary>
        /// Executing this method will perform the operations of the sample using Named tags.
        /// </summary>
        public static void Run()
        {
            // Initialize cache
            InitializeCache();

            // Creating named tag dictionary. 
            NamedTagsDictionary namedTagDict = new NamedTagsDictionary();
            namedTagDict.Add("Category", "SoftDrinks");
            namedTagDict.Add("Discount", 0.25);

            // Add Items in cache with named tags
            AddItems(namedTagDict);

            // Fetch Items from the cache
            //GetItems("1Coke");
            GetData();

            // Dispose cache once done
            //_cache.Dispose();
        }

        /// <summary>
        /// This method initializes the cache.
        /// </summary>
        private static void InitializeCache()
        {
            _cache = CacheManager.GetCache("PRCache");
            _cache.Clear();
            Console.WriteLine("Cache has been initialized!");
        }

        /// <summary>
        /// This method adds items in the cache along with namedTags.
        /// </summary>
        /// <param name="namedTagDict"> Named tags that will be added with the items. </param>
        private static void AddItems(NamedTagsDictionary namedTagDict)
        {
            // Add Name Tags data to Cache
            AddNamedTagDataToCache(1, "Coke", 150, namedTagDict);
            AddNamedTagDataToCache(2, "Pepsi", 150, namedTagDict);

            Console.WriteLine("Items added in cache.");
        }

        private static void AddNamedTagDataToCache(int id, string name, int price, NamedTagsDictionary namedTagDict)
        {
            CacheItem cacheItem = new CacheItem(new Product() { Id = id, Name = name, Price = price });
            cacheItem.NamedTags = namedTagDict;
            _cache.Add(id+name, cacheItem);
        }

        /// <summary>
        /// This method fetches items from the cache using named tags.
        /// </summary>
        private static void GetItems(string key)
        {
           CacheItem item = _cache.GetCacheItem(key);
           
           foreach(var tag in item.NamedTags)
            {
                Console.WriteLine(tag.ToString());
            }
        }

        private static void GetData()
        {
            string query = "SELECT DataModel.Product WHERE this.Discount >= 0.05";
            var queryCommand = new QueryCommand(query);
            ICacheReader result = _cache.SearchService.ExecuteReader(queryCommand, true);
            if (!result.IsClosed)
            {
                Console.WriteLine("==========");
                while (result.Read())
                {
                    Console.WriteLine(result.GetInt32(0));
                    //Console.WriteLine(result.GetValue<Product>(1).Price);
                    Console.WriteLine("==========");
                }
            }
        }
    }
}
