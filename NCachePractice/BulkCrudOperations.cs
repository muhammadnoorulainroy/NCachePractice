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
    public class BulkCrudOperations
    {
        // Instantiate random number generator.  
        private static readonly Random _random = new Random();
        private static ICache cache = null;
        private static string[] _keys = new string[3000];

        public BulkCrudOperations(ICache _cache)
        {
            cache = _cache;
            Initialize();
        }

        private void Initialize()
        {
            for (int i = 0; i < _keys.Length; i++)
            {
                _keys[i] =i.ToString();
            }
        }
        public void RemoveBulk()
        {
            IDictionary<string, User> removedUsers;

            cache.RemoveBulk(_keys, out removedUsers);
            // If items have been returned
            if (removedUsers.Count > 0)
            {
                foreach (KeyValuePair<string, User> entry in removedUsers)
                {
                    if (entry.Value is User)
                    {
                        User user = entry.Value;
                        Console.WriteLine("Removed User is: " + user.Name + "\tAge: " + user.Age);
                    }
                    else
                    {
                        // Object not of Product class
                    }
                }
            }
            else
            {
                // No objects removed
                Console.WriteLine("No objects were removed");
            }
        }

        public void AddBulk()
        {
            IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
            for (int i = 0; i < _keys.Length; i++)
            {
                dictionary.Add(i.ToString(), new CacheItem(new User("Zeeshan", 44)));
            }
            IDictionary<string, Exception> keysFailedToAdd = cache.AddBulk(dictionary);
            if (keysFailedToAdd.Count > 0)
            {
                foreach (KeyValuePair<string, Exception> entry in keysFailedToAdd)
                {
                    if (entry.Value is OperationFailedException)
                    {
                        var exception = entry.Value as OperationFailedException;

                        if (exception.ErrorCode == NCacheErrorCodes.KEY_ALREADY_EXISTS)
                        {
                            // An item with the same key already exists
                            Console.WriteLine("Item " + entry.Key + " Could not be added!. An item with the same key already exists");
                        }
                    }
                    else
                    {
                        // Any other exception
                    }
                }
            }
            else if((dictionary.Count - keysFailedToAdd.Count) > 0)
            {
                Console.WriteLine(((dictionary.Count - keysFailedToAdd.Count) + " Items have been added!"));
            } 
            else
            {
                Console.WriteLine("All Items have been added!");
            }
        }

        public void InsertBulk()
        {
            IDictionary<string, CacheItem> dictionary = new Dictionary<string, CacheItem>();
            for (int i = 0; i < _keys.Length; i++)
            {
                dictionary.Add(i.ToString(), new CacheItem(new User("Zeeshan", 44)));
            }
            IDictionary<string, Exception> keysFailedToUpdate = cache.InsertBulk(dictionary);
            if (keysFailedToUpdate.Count > 0)
            {
                foreach (KeyValuePair<string, Exception> entry in keysFailedToUpdate)
                {
                    if (entry.Value is OperationFailedException)
                    {
                        var exception = entry.Value as OperationFailedException;

                        if (exception.ErrorCode == NCacheErrorCodes.KEY_ALREADY_EXISTS)
                        {
                            // An item with the same key already exists
                            Console.WriteLine("Item " + entry.Key + " Could not be inserted!. An item with the same key already exists");
                        }
                    }
                    else
                    {
                        // Any other exception
                    }
                }
            }
            else if ((dictionary.Count - keysFailedToUpdate.Count) > 0)
            {
                Console.WriteLine(((dictionary.Count - keysFailedToUpdate.Count) + " Items have been inserted!"));
            }
            else
            {
                Console.WriteLine("All Items have been inserted!");
            }
        }
    }
}
