using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using DataModel;

namespace NCachePractice
{
    public class SimpleMultiThreadedCrudOperations
    {
        // Instantiate random number generator.  
        private static readonly Random _random = new Random();
        private static ICache cache = null;
        private static int[] _keys = new int[3000];

        public SimpleMultiThreadedCrudOperations(ICache _cache) { 
            cache = _cache; 
            Initialize(); 
        }

        private void Initialize()
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] = RandomNumber(1, 1000000);
            }
        }
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        public  void AddSync()
        {
            Console.WriteLine("\nAdding Data Synchronously.....");
            for (int i = 0; i < 2000; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Create a CacheItem 
                        var cacheItem = new CacheItem(new User("Umar", i));
                        // Add User object to cache
                        cache.Add(_keys[i].ToString(), cacheItem);

                    }
                    catch (OperationFailedException ex)
                    {
                        // NCache specific exception

                        if (ex.ErrorCode == NCacheErrorCodes.KEY_ALREADY_EXISTS)
                        {
                            // An item with the same key already exists
                            //Console.WriteLine("An item with the same key already exists");
                        }
                        else
                        {
                            // Exception can occur due to:
                            // Connection Failures 
                            // Operation Timeout
                            // Operation performed during state transfer
                            Console.WriteLine(ex.Message);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }

                }).Start();
            }
            
        }

        public void AddAsync()
        {
            Console.WriteLine("\nAdding Data Asynchronously.....");
            for (int i = 0; i < 2000; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Create a CacheItem 
                        var cacheItem = new CacheItem(new User("Umar", i));
                        // Add User object to cache
                        Task task = cache.AddAsync(_keys[i].ToString(), cacheItem);
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("Task completed due to an unhandled exception");
                        }
                        else if (task.IsCanceled)
                        {
                            Console.WriteLine("Task Canceled!");
                        }
                        else if (task.IsCompleted)
                        {
                            Console.WriteLine("Task Completed!");
                        }
                    }
                    catch (OperationFailedException ex)
                    {
                        // NCache specific exception
                        // Exception can occur due to Connection Failure
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }

                }).Start();
            }

        }

        public void InsertSync()
        {
            Console.WriteLine("\nInserting Data Synchronously.....");
            for (int i = 0; i < 2000; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Create a CacheItem 
                        var cacheItem = new CacheItem(new User("Umar", i));
                        // Add User object to cache
                        CacheItemVersion version = cache.Insert(_keys[i].ToString(), cacheItem);

                    }
                    catch (OperationFailedException ex)
                    {
                        // Exception can occur due to:
                        // Connection Failures 
                        // Operation Timeout
                        // Operation performed during state transfer
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }

                }).Start();
            }
        }

        public void InsertAsync()
        {
            Console.WriteLine("\nInserting Data Asynchronously.....\n");
            for (int i = 0; i < 2000; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        // Create a CacheItem 
                        var cacheItem = new CacheItem(new User("Umar", i));
                        // Add User object to cache

                        Task task = cache.InsertAsync(_keys[i].ToString(), cacheItem);

                        //This task object can be used as per your business needs
                        if (task.IsFaulted)
                        {
                            // Task completed due to an unhandled exception
                            Console.WriteLine("Task faulted due to an unhandled exception");
                        }
                        else if (task.IsCanceled)
                        {
                            Console.WriteLine("Task canceled");
                        }
                        else if (task.IsCompleted)
                        {
                            Console.WriteLine("Task Completed Successfully");
                        }
                    }
                    catch (OperationFailedException ex)
                    {
                        // NCache specific exception
                        // Exception can occur due to Connection Failure
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }
                }).Start();
            }
        }

        public void GetSync()
        {
            Console.WriteLine("\nGetting Data........\n");
            for (int j = 0; j < 2000; j++)
            {
                new Thread(() =>
                {
                    try
                    {
                        var retrievedItem = cache.GetCacheItem(_keys[j].ToString());
                        if (retrievedItem != null)
                        {

                            Console.WriteLine(retrievedItem.GetValue<User>().Name + "\t" + retrievedItem.GetValue<User>().Age);
                        }
                        else
                        {
                            // Null returned; no key exists
                            Console.WriteLine("Item not found");
                        }
                    }
                    catch (OperationFailedException ex)
                    {
                        // Exception can occur due to:
                        // Connection Failures 
                        // Operation Timeout
                        // Operation performed during state transfer
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }

                }).Start();
            }
        }

        public void RemoveSync()
        {
            Console.WriteLine("\nRemoving Data Synchronously.......");
            for (int j = 0; j < 2000; j++)
            {
                new Thread(() =>
                {
                    try
                    {
                        User userRemoved = new User();

                        cache.Remove(_keys[j].ToString(), out userRemoved);
                        if (userRemoved != null)
                        {
                            if (userRemoved is User)
                            {
                                Console.WriteLine("User Removed!\tName = {0} \tAge = {1}  " , userRemoved.Name, userRemoved.Age);
                            }
                            else
                            {
                                // The object removed was not of User type
                                // Add it back to the cache
                                CacheItemVersion ver = cache.Add(_keys[j].ToString(), userRemoved);
                            }
                        }
                        else
                        {
                            // Item does not exist in cache
                            Console.WriteLine("Could not remove item "+ _keys[j].ToString() + "! Item does not exist in cache");
                        }
                    }
                    catch (OperationFailedException ex)
                    {
                        // Exception can occur due to:
                        // Connection Failures 
                        // Operation Timeout
                        // Operation performed during state transfer
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }

                }).Start();
            }
        }

        public void RemoveAsync()
        {
            Console.WriteLine("\nRemoving Data Asynchronously.......");
            for (int j = 0;j <2000; j++)
            {
                new Thread(() =>
                {
                    try
                    {
                        Task task = cache.RemoveAsync<User>(_keys[j].ToString());
                        //This task object can be used as per your business needs
                        if (task.IsFaulted)
                        {
                            // Task completed due to an unhandled exception
                            Console.WriteLine("Task completed due to an unhandled exception");
                        }
                        else if (task.IsCanceled)
                        {
                            Console.WriteLine("Task canceled");
                        }
                        else if (task.IsCompleted)
                        {
                            Console.WriteLine("Task Completed Successfully");
                        }
                    }
                    catch (OperationFailedException ex)
                    {
                        // Exception can occur due to:
                        // Connection Failures 
                        // Operation Timeout
                        // Operation performed during state transfer
                        Console.WriteLine(ex.Message);
                    }
                    catch (Exception ex)
                    {
                        // Any generic exception like ArgumentNullException or ArgumentException
                        Console.WriteLine(ex.Message);
                    }
                }).Start();
            }
        }
    }
}
