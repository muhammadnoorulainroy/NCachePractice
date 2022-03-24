using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Runtime.Events;
using Alachisoft.NCache.Runtime.Exceptions;
using Alachisoft.NCache.Client;
using DataModel;

namespace NCachePractice
{
    public class ContinousQuery
    {
        private static ICache cache;
        public static void Run()
        {
            Initialize();
            cache.Clear();
            RegisterQueryAndNotification();
            MakeChangesInCache();
        }

        private static void Initialize()
        {
            cache = CacheManager.GetCache("PRCache");
        }


        private static void MakeChangesInCache()
        {
            for(int i=95; i<110; i++)
            {
                UpdateData(new Product { Id=i, Name="Coffee", Price=i});
                Thread.Sleep(1000);
            }
        }
        static void QueryItemCallBack(string key, CQEventArg arg)
        {
            switch (arg.EventType)
            {
                case EventType.ItemAdded:
                    // "key" has been added to cache
                    Console.WriteLine("Item has been added! " + arg.Item.GetValue<Product>().Name);
                    break;

                case EventType.ItemUpdated:
                    // "key" has been updated in cache
                    // Get updated Product object
                    if (arg.Item != null)
                    {
                        Product updatedProduct = arg.Item.GetValue<Product>();
                        Console.WriteLine(updatedProduct.Name + " Product has been updated!!");
                    }
                    break;

                case EventType.ItemRemoved:
                    // "key" has been removed from cache
                    Console.WriteLine("Product has been removed!!!");
                    break;
            }
        }

        private static void RegisterQueryAndNotification()
        {
            try
            {
                // Query for required operation
                string query = "SELECT DataModel.Product WHERE this.Price > ? AND this.Price < 105";

                var queryCommand = new QueryCommand(query);
                queryCommand.Parameters.Add("Price", 100);

                // Create Continuous Query
                var cQuery = new ContinuousQuery(queryCommand);

                // Item add notification 
                // EventDataFilter.None returns the cache keys added
                cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallBack), EventType.ItemAdded, EventDataFilter.DataWithMetadata);

                // Item update notification 
                // EventDataFilter.DataWithMetadata returns cache keys + modified item + metadata on updation
                cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallBack), EventType.ItemUpdated, EventDataFilter.DataWithMetadata);

                // Item Remove notification 
                // EventDataFilter.Metadata returns cache keys + item metadata on updation
                cQuery.RegisterNotification(new QueryDataNotificationCallback(QueryItemCallBack), EventType.ItemRemoved, EventDataFilter.DataWithMetadata);

                // Register continuousQuery on server 
                cache.MessagingService.RegisterCQ(cQuery);

                // Query Cached Data
                ICacheReader reader = cache.SearchService.ExecuteReader(queryCommand);

                // If resultset is not empty
                if (reader.FieldCount > 0)
                {
                    while (reader.Read())
                    {
                        Product result = reader.GetValue<Product>(1);
                        // Perform operations

                    }
                }
                else
                {
                    // Null query result set returned
                }
                
            }
            catch (OperationFailedException ex)
            {
                if (ex.ErrorCode == NCacheErrorCodes.INCORRECT_FORMAT)
                {
                    // Make sure that the query format is correct
                    Console.WriteLine("Make sure that the query format is correct\n" + ex.Message);
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
                // Any generic exception like ArgumentException, ArgumentNullException
                Console.WriteLine(ex.Message);
            }

        }

        private static void UpdateData(Product product)
        {
            string key = $"Product:(product.Id)";

            cache.Insert(key, product);
        }

        private static void AddData(Product product)
        {
            string key = $"Product:(product.Id)";

            cache.Add(key, product);
        }

        private static void RemoveData(Product product)
        {
            string key = $"Product:(product.Id)";

            cache.Remove(key);
        }

    }
}
