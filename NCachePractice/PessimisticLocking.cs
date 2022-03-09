using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;

namespace NCachePractice
{
    public class PessimisticLocking
    {
        private static ICache _cache=null;
        public static void Run()
        {
            // Initialize cache            
            InitializeCache();

            string key = "1";

            // Add item in cache
            AddItemInCache(key);

            // Create new lock handle to fetch Item usin locking
            LockHandle lockHandle = new LockHandle();

            // Timespan for which lock will be taken
            TimeSpan timeSpan = new TimeSpan(0, 0, 0, 20);

            // Get item from cache
            //GetItemFromCache(key, lockHandle, timeSpan);

            // Lock Item in cache 
            LockItemInCache(key, lockHandle, timeSpan);

            // Unlock item in cache using multiple ways
            UnLockItemInCache(key, lockHandle);

            RemoveItemFromCache(key);

            // Dispose cache once done
            _cache.Dispose();

            Console.ReadKey();  
        }

        /// <summary>
        /// This method initializes the cache.
        /// </summary>
        private static void InitializeCache()
        {
            //string cache = ConfigurationManager.AppSettings["CacheId"];

            /*if (String.IsNullOrEmpty(cache))
            {
                Console.WriteLine("The Cache Name cannot be null or empty.");
                return;
            }*/

            // Initialize an instance of the cache to begin performing operations:
            _cache = CacheManager.GetCache("ClusteredCache-2");
            Console.WriteLine("Cache initialized successfully");
            _cache.Clear();
        }
        /// <summary>
        /// This method add items in the cache.
        /// </summary>
        private static void AddItemInCache(string key)
        {
            //Adding an item the cache
           User user = new User("Zeeshan", 44);

            _cache.Add(key, user);
        }

        private static void RemoveItemFromCache(string key)
        {
            _cache.Remove(key);
        }

        /// <summary>
        /// This method fetches item from the cache.
        /// </summary>
        /// <param name="lockHandle"> An instance of lock handle that will be used for locking the fetched item. </param>
        /// <param name="timeSpan"> Time for which the lock will be held. </param>
        private static void GetItemFromCache(string key, LockHandle lockHandle, TimeSpan timeSpan)
        {
            // GetT Get<T> (string key, bool acquireLock, TimeSpan lockTimeout, ref LockHandle lockHandle);
            User getUser = _cache.Get<User>(key, true, timeSpan, ref lockHandle);

            PrintCustomerDetails(getUser);
            Console.WriteLine("Lock acquired on " + lockHandle.LockId);
        }

        /// <summary>
        /// This method locks specified item in the cache
        /// </summary>
        /// <param name="lockHandle"> Handle of the lock. </param>
        /// <param name="timeSpan"> Time for which lock will be held. </param>
        private static void LockItemInCache(string key, LockHandle lockHandle, TimeSpan timeSpan)
        {
            // Lock item in cache
            bool isLocked = _cache.Lock(key, timeSpan, out lockHandle);
            Console.WriteLine("is Locked => " + isLocked);
            if (!isLocked)
            {
                Console.WriteLine("Lock acquired on " + lockHandle.LockId);
            }
        }

        /// <summary>
        /// This method unlocks the item in the cache using 2 different ways.
        /// </summary>
        /// <param name="lockHandle"> The lock handle that was used to lock the item. </param>
        private static void UnLockItemInCache(string key, LockHandle lockHandle)
        {
            _cache.Unlock(key, lockHandle);

            //Forcefully unlock item in cache 
            //_cache.Unlock("Customer:KirstenGoli");
        }

        /// <summary>
        /// Method for printing user type details.
        /// </summary>
        /// <param name="user"></param>
        public static void PrintCustomerDetails(User user)
        {
            Console.WriteLine();
            Console.WriteLine("User Details are as follows: ");
            Console.WriteLine("Name: " + user.Name);
            Console.WriteLine("Age:  " + user.Age);
            
        }
    }
}
