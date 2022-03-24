using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using Alachisoft.NCache.Client;
using System.Configuration;
using DataModel;
namespace NCachePractice
{
    public class Events
    {
        private static ICache cache;
        private static CacheEventDescriptor eventDescriptor;
        private static int[] _keys = new int[3000];
        private static Random _random;

        
        public static void Run()
        {
            
            //Initialize Cache
            Initialize();
            //RegisterGeneralCacheNotification();
            
            //Console.WriteLine("Key[0]: " + _keys[0]);
            for (int i=0; i<20; i++)
            {
                AddItem(_keys[i].ToString());
            }
            RegisterSelectiveCacheNotification(_keys[0].ToString());
            for (int i = 0; i < 20; i++)
            {
                UpdateItem(_keys[i].ToString());
            }
            for (int i = 0; i < 20; i++)
            {
                RemoveItem(_keys[i].ToString());
            }
            UnregisterSelectiveCacheNotification(_keys[0].ToString());
            for (int i = 0; i < 20; i++)
            {
                AddItem(_keys[i].ToString());
            }
            for (int i = 0; i < 20; i++)
            {
                UpdateItem(_keys[i].ToString());
            }
            for (int i = 0; i < 20; i++)
            {
                RemoveItem(_keys[i].ToString());
            }
            Console.ReadKey();
        }
        public static void Initialize()
        {
            cache = CacheManager.GetCache("ClusteredCache-2");
            
            Console.WriteLine("Cache has been initialized!");
            cache.NotificationService.CacheStopped += OnCacheStopped;
            cache.NotificationService.MemberJoined += OnMemberJoined;
            cache.NotificationService.MemberLeft += OnMemberLeft;
            cache.NotificationService.CacheCleared += OnCacheCleared;
            cache.Clear();
            _random  = new Random();
            for (int i = 0; i < _keys.Length; i++)
            { 
                int random = RandomNumber(1, 1000000);
                if (!_keys.Contains(random))
                {
                    _keys[i] = random;
                }
            }
        }
        public static int RandomNumber(int min, int max)
        {
            return _random.Next(min, max);
        }

        private static void RegisterGeneralCacheNotification() {
            eventDescriptor = cache.MessagingService.RegisterCacheNotification(new CacheDataNotificationCallback(CacheDataModified),
                EventType.ItemAdded | EventType.ItemRemoved | EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
            if(eventDescriptor.IsRegistered)
                Console.WriteLine("Cache Notification registered successfully.");
            else
                Console.WriteLine("Cache notification could not register");
        }

        private static void CacheDataModified(string key, CacheEventArg eventArg)
        {

            switch (eventArg.EventType)
            {
                case EventType.ItemAdded:
                    Console.WriteLine("\nItem added.\nKey {0}\nValue {1}", key, eventArg.Item.GetValue<User>().Name);
                    break;
                case EventType.ItemRemoved:
                    Console.WriteLine("\nItem Removed\nKey {0}\nValue {1}", key, eventArg.Item.GetValue<User>().Name);
                    break;
                case EventType.ItemUpdated:
                    Console.WriteLine("\nItem Updated\nOld Value: Key {0} \t Value {1}", key, eventArg.OldItem.GetValue<User>().Name);
                    Console.WriteLine("\nItem Updated\nNew Value: Key {0} \t Value {1}", key, eventArg.Item.GetValue<User>().Name);
                    break;
            }
        }

        private static void RegisterSelectiveCacheNotification(string key)
        {
            cache.MessagingService.RegisterCacheNotification(key, new CacheDataNotificationCallback(CacheDataModified),
                EventType.ItemAdded | EventType.ItemRemoved | EventType.ItemUpdated, EventDataFilter.DataWithMetadata);
        }

        private static void UnregisterSelectiveCacheNotification(string key)
        {
            cache.MessagingService.UnRegisterCacheNotification(key, new CacheDataNotificationCallback(UnregisterSelectiveNotificationCallback),
               EventType.ItemAdded);
        }

        private static void UnregisterSelectiveNotificationCallback(string key, CacheEventArg eventArg)
        {
            Console.WriteLine("Unregistered Notifications against the key " + key + " for Event Type " + eventArg.EventType.ToString());
        }
        
        //Add Item in cache
        private static void AddItem(string key)
        {
            CacheItem cacheItem = new CacheItem(new User(1, "Zeeshan", 24));
            cache.Add(key, cacheItem);
        }

        //Update Item
        private static void UpdateItem(string key)
        {
            CacheItem cacheItem = new CacheItem(new User(1, "Umar", 24));
            cache.Insert(key, cacheItem);
        }

        //Remove Item
        private static void RemoveItem(string key)
        {
            cache.Remove(key);
        }

        public static void OnCacheCleared()
        {
            Console.WriteLine("Cache has been Cleared!!!!");
        }
        public static void OnCacheStopped(string cacheName)
        {
            Console.WriteLine("Cache " + cacheName + " Stopped!!!!");
        }

        public static void OnMemberJoined(NodeInfo nodeInfo)
        {
            // Perform task after Member Joined event gets fired
            Console.WriteLine("Node " + nodeInfo.IpAddress + " Joined");
        }

        public static void OnMemberLeft(NodeInfo nodeInfo)
        {
            Console.WriteLine("Node " + nodeInfo.IpAddress + " Left");
            // Perform task after Member Left event gets fired
        }
    }
}
