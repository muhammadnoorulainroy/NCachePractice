using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Dependencies;
using System.Collections;
using System.Configuration;
using DataModel;


namespace LoaderAndRefresher
{
    public class UsageOfCacheLoaderAndRefresher
    {
        private static ICache cache;

       
        public static void Run()
        {
            // Initialize the cache
            InitializeCache();
            
            // Read data from cache
            ReadObjectsFromCache();
        }

        private static void InitializeCache()
        {
            string cacheName = "ClusteredCache";

            if (String.IsNullOrEmpty(cacheName))
            {
                Console.WriteLine("The Cache Name cannot be null or empty.");
                return;
            }

            // Initialize an instance of the cache to begin performing operations:
            cache = CacheManager.GetCache(cacheName);
            // Print output on console
            Console.WriteLine(string.Format("\nCache '{0}' is initialized.", cacheName));
        }

        private static void ReadObjectsFromCache()
        {

            long count = cache.Count;
            Console.WriteLine("\nCache Count: " + count);

            User cachedCustomer = null;
            Product cachedProduct = null;

            foreach (DictionaryEntry cacheEntry in cache)
            {
                try
                {
                    object obj = cache.Get<object>(cacheEntry.Key as string);
                    if (obj is User)
                    {
                        cachedCustomer = cache.Get<User>(cacheEntry.Key as string) as User;
                        printCustomerDetails(cachedCustomer);
                    }
                    else if (obj is Product)
                    {
                        cachedProduct = cache.Get<Product>(cacheEntry.Key as string) as Product;
                        printProductDetails(cachedProduct);
                    }
                    Console.WriteLine("===================================================");
                }
                catch (Exception ex) { 
                    /*handle exception here.*/
                    Console.WriteLine(ex.Message); 
                }
            }
            
        }

        /// <summary>
        /// This method prints detials of supplier type.
        /// </summary>
        /// <param name="cachedCustomer"></param>
        private static void printCustomerDetails(User cachedCustomer)
        {
            if (cachedCustomer == null) return;

            Console.WriteLine();
            Console.WriteLine("Customer Details are as follows: ");
            Console.WriteLine("ID:   " + cachedCustomer.Id);
            Console.WriteLine("Name: " + cachedCustomer.Name);
            Console.WriteLine();
        }

        private static void printProductDetails(Product cachedProduct)
        {
            if (cachedProduct == null) return;

            Console.WriteLine();
            Console.WriteLine("Product Details are as follows: ");
            Console.WriteLine("ID:   " + cachedProduct.Id);
            Console.WriteLine("Name: " + cachedProduct.Name);
            Console.WriteLine();
        }
    }
}
