using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using DataModel;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Dependencies;
using System.Configuration;

namespace Dependency
{
    public class FileBasedDependency
    {
        private static ICache cache;

        public static void Run()
        {
            string file = "C:\\Users\\noor_ul_ain\\Desktop\\Dependency file.txt";
            Initialize();
            cache.Clear();
            AddDataToCacheWithFileDependency(file);
            //Console.WriteLine("Before modifying the dependency file");
            //GetItem("1");
           /* AddItemsWithDependencies();
            GetItem("2");
            // Thread.Sleep(4000);
            UpdateItem();
            GetItem("2");*/

            //AddDependency("C:\\Users\\noor_ul_ain\\Desktop\\Dependency file.txt", "1");

          /* ModifyDependencyFile(file);
            Console.WriteLine("\nAfter modifying the dependency file");
            GetItem("1");*/
        }

        private static void Initialize()
        {
            cache = CacheManager.GetCache("PRCache");
            Console.WriteLine("Cache has been initialized!!!!\n");
        }

        private static void AddDataToCacheWithFileDependency(string file)
        {
            // create a new user and set the dependency
            var user = new User(1, "Umar", 24);
            CacheItem cacheItem = new CacheItem(user);

            cacheItem.Dependency = new FileDependency(file, DateTime.Now.AddMinutes(1));
            cache.Insert(user.Id.ToString(), cacheItem);

            cache.Add(user.Id.ToString(), cacheItem);
            Console.WriteLine("User Added\n");
        }

        //Set Dependency on already existing item
        private static void AddDependency(string file, string userKey)
        {
            var dependency = new FileDependency(file);
            var attr = new CacheItemAttributes();
            attr.Dependency = dependency;
            cache.UpdateAttributes(userKey, attr);
        }


        private static void GetItem(string key)
        {
            User user = cache.Get<User>(key);
            if (user != null)
            {
                Console.WriteLine(user.Id + "\t" + user.Name + "\t" + user.Age);
            }
            else
            {
                Console.WriteLine("Key does not exist");
            }
        }

        private static void ModifyDependencyFile(string path)
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                writer.WriteLine(string.Format("\n{0}\tFile is modifed. ", DateTime.Now));
            }
        }

        
    }
}
