using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataModel;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Dependencies;
using System.Configuration;


namespace Dependency
{
    public class KeyBasedDependency
    {
        private static ICache cache;

        public static void Run()
        {
            // Initialize Cache
            Initialize();

            //cache.Clear();

            //Add Items in cache with dependencies
            //AddItemsWithDependencies();

            //Add Items in cache without dependencies
            //AddItemsWithoutDependencies();

            //get item from cache
            //GetItem("2");

            //Add Items with multiple depedencies
            AddItemsWithMultipleDependencies();

            //get item from cache
            //GetItem("2");
            Console.WriteLine("\n\nBefore Updating\n\n");
            for (int i = 3; i < 9; i++)
            {
                GetItem(i.ToString());
            }
            


            //Set Dependency to already existing item
            //AddKeyDependencyToExistingItem("1", "2");

            //get item from cache
            //GetItem("2");

            //Update items in cache
             UpdateItem();

            Console.WriteLine("\n\nAfter Updating\n\n");
            //get item from cache
            //GetItem("2");

            for (int i = 3; i < 9; i++)
            {
                GetItem(i.ToString());
            }
        }

        private static void Initialize()
        {
            cache = CacheManager.GetCache("PRCache");
            Console.WriteLine("Cache has been initialized\n");
        }

        private static void AddItemsWithDependencies()
        {
            Product product = new Product { Id = 1, Name = "Coffee", Price = 140 };
            Order order = new Order { Id = 2, Price = 900, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            cache.Insert(product.Id.ToString(), product);

            var cacheItem = new CacheItem(order);
            cacheItem.Dependency = new KeyDependency(product.Id.ToString());
            cache.Insert(order.Id.ToString(), cacheItem);
            Console.WriteLine("Order id: " + order.Id + " Added with dependency on Product id: " + product.Id);
        }

        //Add items with multiple dependencies
        private static void AddItemsWithMultipleDependencies()
        {
            Product product = new Product { Id = 1, Name = "Coffe", Price = 140 };
            User user = new User { Id = 2, Name = "Noor", Age = 24 };
            cache.Insert(product.Id.ToString(), product);
            cache.Insert(user.Id.ToString(), user);
            
            /*Order order = new Order  { Id = 3, Price = 900, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            Order order2 = new Order { Id = 4, Price = 400, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            Order order3 = new Order { Id = 5, Price = 500, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            Order order4 = new Order { Id = 6, Price = 600, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            Order order5 = new Order { Id = 7, Price = 700, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            Order order6 = new Order { Id = 8, Price = 800, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };*/

            for(int i = 3; i < 9; i++)
            {
                Order order = new Order { Id = i, Price = i*100, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
                var cacheItem = new CacheItem(order);
                cacheItem.Dependency = new KeyDependency(new string[2] { product.Id.ToString(), user.Id.ToString() });
                cache.Insert(order.Id.ToString(), cacheItem);
            }
            //Console.WriteLine("Order id: " + order.Id + " Added with dependency on Product id: " + product.Id + " and User Id: " + user.Id);
        }

        private static void AddItemsWithoutDependencies()
        {
            Product product = new Product { Id = 1, Name = "Coffe", Price = 140 };
            Order order = new Order { Id = 2, Price = 900, ShippingAddress = "Diya Technologies, G10/4, Islamabad" };
            cache.Insert(product.Id.ToString(), product);

            var cacheItem = new CacheItem(order);
            cache.Insert(order.Id.ToString(), cacheItem);
            Console.WriteLine("Order id: " + order.Id + " and Product id: " + product.Id + " Added");
        }

        private static void UpdateItem()
        {
            Product product = new Product { Id = 1, Name = "Coffee", Price = 150 };
            cache.Insert(product.Id.ToString(), product);
            User user = new User { Id = 2, Name = "Noor", Age = 40 };
            cache.Insert(user.Id.ToString(), user);
        }

        private static void UpDateItems()
        {
            Order order  = new Order { Id = 1, Price = 900, ShippingAddress = "E11/2, Islamabad" };
            Order order2 = new Order { Id = 2, Price = 1000, ShippingAddress = "Lahore" };
            Order order3 = new Order { Id = 3, Price = 1500, ShippingAddress = "Comsats" };
            User user = new User { Id = 2, Name = "Zeeshan", Age = 100, Group = "Software Engineer" };
            cache.Insert(order.Id.ToString(), order);
            cache.Insert(order2.Id.ToString(), order2);
            cache.Insert(order3.Id.ToString(),order3);
            cache.Insert(user.Id.ToString(),user);
        }

        private static void GetItem(string key)
        {
            Order order = cache.Get<Order>(key);
            if(order != null)
            {
                Console.WriteLine(order.Id + "\t" + order.Price + "\t" + order.ShippingAddress);
            }
            else
            {
                Console.WriteLine("Key does not exist");
            }
        }

        private static void AddKeyDependencyToExistingItem(string productKey, string orderKey)
        {
            var dependency = new KeyDependency(productKey);

            var attr = new CacheItemAttributes();
            attr.Dependency = dependency;
            cache.UpdateAttributes(orderKey, attr);
        }
    }
}
