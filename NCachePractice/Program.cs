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
    internal class Program
    {

        // Instantiate random number generator.  
        private static readonly Random _random = new Random();
        private static int[] _keys = new int[3000];
        private static User[] _users = new User[3000];
        private static ICache cache=null;
        // Generates a random number within a range.      
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }
        
        static void Main(string[] args)
        {
            //PessimisticLocking.Run();
            //Streaming.Run();
            //Groups.Run();
            //Tags.Run();
            //NamedTags.Run();
            //Events.Run();
            PubSub.Run();
            //ContinousQuery.Run();
            // Specify the cache name
            //string cacheName = "ClusteredCache-2";

            // Connect to cache
            //cache = CacheManager.GetCache(cacheName);
            //var crudOperations = new SimpleMultiThreadedCrudOperations(cache);
            //var bulkOperations = new BulkCrudOperations(cache);
            //cache.Clear();
            /*crudOperations.AddSync();
            crudOperations.GetSync();
            crudOperations.InsertSync();
            crudOperations.GetSync();
            crudOperations.RemoveSync();
            bulkOperations.AddBulk();
            bulkOperations.InsertBulk();
            bulkOperations.RemoveBulk();*/


            Console.ReadKey();
            //ShowMenu();

        }

        private static void ShowMenu()
        {
            bool isActive = true;

            while (isActive)
            {
                Thread.Sleep(500);
                Console.WriteLine("\n======= Main Menu =======\n");

                Console.WriteLine("1. ADD User in Cache");
                Console.WriteLine("2. INSERT User in Cache");
                Console.WriteLine("3. REMOVE User from Cache");
                Console.WriteLine("4. GET Data from Cache");
                Console.WriteLine("5. CLEAR Cache Data");
                Console.WriteLine("6. DISPOSE");


                Console.Write("Enter your choice: ");

                string inputString = Console.ReadLine();


                if (Int32.TryParse(inputString, out int input))
                {
                    switch (input)
                    {
                        case 1:
                            Add();
                            break;
                        case 2:
                            Insert();
                            break;
                        case 3:
                            Remove();
                            break;
                        case 4:
                            var user = Get();
                            if (user != null)
                            {
                                Console.WriteLine("User Name = {0}\nUser Age = {1}", user.Name, user.Age);
                            }
                            else
                            {
                                Console.WriteLine("Key not found");
                            }
                            break;
                        case 5:
                            cache.Clear();
                            break;
                        case 6:
                            cache.Dispose();
                            isActive = false;
                            break;
                        
                        default:
                            Console.WriteLine("Please enter number from the given choices");
                            break;
                    }
                }
                else
                    Console.WriteLine("Please enter valid input\n");
            }
        }
        private static void Add()
        {
            string key;
            User user = new User();

            Console.Write("\nEnter key:");
            key = Console.ReadLine();

            Console.Write("\nEnter Customer Name:");
            user.Name = Console.ReadLine();

            Console.Write("\nEnter Customer Age:");
            var ageAsString = Console.ReadLine();
            int age;
            while (!int.TryParse(ageAsString, out age))
            {
                Console.WriteLine("Age must be a number!");
                Console.Write("\nEnter Customer Age:");
                ageAsString = Console.ReadLine();
            }
            user.Age = age;

           CacheItemVersion itemVersion =  cache.Add(key, user);
           Console.WriteLine(itemVersion.Version);
        }

        private static void Insert()
        {
            string key;
            User user = new User();

            Console.Write("\nEnter key:");
            key = Console.ReadLine();

            Console.Write("\nEnter Customer Name:");
            user.Name = Console.ReadLine();

            Console.Write("\nEnter Customer Age:");
            var ageAsString = Console.ReadLine();
            int age;
            while (!int.TryParse(ageAsString, out age))
            {
                Console.WriteLine("Age must be a number!");
                Console.Write("\nEnter Customer Age:");
                ageAsString = Console.ReadLine();
            }
            user.Age = age;
            LockHandle lockHandle = null;
            var lockSpan = TimeSpan.FromSeconds(10);
            bool lockAcquired = cache.Lock(key, TimeSpan.Zero, out lockHandle);
            Console.WriteLine(lockAcquired);
            
            if (lockAcquired)
            {
                CacheItemVersion itemVersion = cache.Insert(key, user);
                Console.WriteLine(itemVersion.Version);
                Console.ReadKey();
                //cache.Unlock(key, lockHandle);
            }
            else if(cache.Contains(key) && !lockAcquired)
            {
                Console.WriteLine("Item is already locked!");
            }
            else
            {
                Console.WriteLine("Key does not exist to lock");
            }
            
        }

        private static void Remove()
        {
            string key;
            Console.Write("\nEnter key:");
            key = Console.ReadLine();
            cache.Remove(key);
        }

        private static User Get()
        {
            string key;
            Console.Write("\nEnter key:");
            key = Console.ReadLine();
            bool acquireLock = true;

            // Specify time span of 10 seconds for which the item remains locked
            TimeSpan lockSpan = TimeSpan.FromSeconds(10);

            //Create a new LockHandle
            LockHandle lockHandle = null;

            // Lock the item for a time span of 10 seconds
            var result = cache.Get<User>(key, acquireLock, lockSpan, ref lockHandle);

            // Verify if the item is locked successfully
            if (result != null)
            {
                // Item has been successfully locked
                cache.Unlock(key, lockHandle);
                
                return result;
            }
            else
            {
                // Key does not exist
                // Item is already locked with a different LockHandle
                return null;
            }
           // return cache.Get<User>(key);
        }
    }
}
