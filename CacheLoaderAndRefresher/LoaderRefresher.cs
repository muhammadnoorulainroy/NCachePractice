using System;
using System.Collections.Generic;
using Alachisoft.NCache.Runtime.CacheLoader;
using System.Data.SqlClient;
using Alachisoft.NCache.Client;
using DataModel;

namespace CacheLoaderAndRefresher
{
    public class LoaderRefresher : ICacheLoader
    {
        private string connectionString;
        private static ICache cache;
        public void Init(IDictionary<string, string> parameters, string cacheName)
        {
            System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();

            /*if (parameters != null && parameters.Count > 0)
                connectionString = parameters.ContainsKey("ConnectionString")
                    ? parameters["ConnectionString"] as string : string.Empty;*/
            connectionString = @"Data Source=NOOR-UL-AIN\SQLEXPRESS;Initial Catalog=CashAndCarry;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User ID=DIYATECH\noor_ul_ain";

            cache = CacheManager.GetCache("Dummy");
            cache.Clear();
        }

        public object LoadDatasetOnStartup(string dataset)
        {
            
            // Create a list of datasets to load at cache startup
            IList<object> datasetToLoad;

            if (string.IsNullOrEmpty(dataset))
                throw new InvalidOperationException("Invalid dataset.");

            switch (dataset.ToLower())
            {
                case "product":
                    datasetToLoad = FetchProductsFromDataSource();
                    break;
                case "customer":
                    datasetToLoad = FetchCustomersFromDataSource();
                    break;
                default:
                    throw new InvalidOperationException("Invalid Dataset.");
            }


            string[] keys = GetKeys(datasetToLoad);


            IDictionary<string, CacheItem> cacheData = GetCacheItemDictionary(keys, datasetToLoad);

            Console.WriteLine("Cache data length: {0}", cacheData.Count);


            var errors = cache.InsertBulk(cacheData);
            foreach (var error in errors)
            {
                Console.WriteLine(error.Value.Message);
            }
            // User context is the time at which datasets were loaded in the cache
            object userContext = DateTime.Now;
            return userContext;
        }

        public object RefreshDataset(string dataSet, object userContext)
        {
            if (string.IsNullOrEmpty(dataSet))
                throw new InvalidOperationException("Invalid dataset.");

            DateTime? lastRefreshTime;

            switch (dataSet.ToLower())
            {
                case "product":
                    lastRefreshTime = userContext as DateTime?;
                    IList<Product> productsNeedToRefresh = FetchUpdatedProducts(lastRefreshTime) as IList<Product>;
                    foreach (var product in productsNeedToRefresh)
                    {
                        string key = $"ProductID:{product.Id}";
                        CacheItem cacheItem = new CacheItem(product);
                        cache.Insert(key, cacheItem);
                    }
                    break;
                case "customer":
                    lastRefreshTime = userContext as DateTime?;
                    IList<User> customerNeedToRefresh = FetchUpdatedCustomers(lastRefreshTime) as IList<User>;
                    foreach (var customer in customerNeedToRefresh)
                    {
                        string key = $"CustomerID:{customer.Id}";
                        CacheItem cacheItem = new CacheItem(customer);
                        cache.Insert(key, cacheItem);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid Dataset.");
            }

            userContext = DateTime.Now;
            return userContext;
        }

        public IDictionary<string, RefreshPreference> GetDatasetsToRefresh(IDictionary<string, object> userContexts)
        {
            IDictionary<string, RefreshPreference> DatasetsNeedToRefresh = new Dictionary<string, RefreshPreference>();
            DateTime? lastRefreshTime;
            bool datasetHasUpdated;

            foreach (var dataSet in userContexts.Keys)
            {
                switch (dataSet.ToLower())
                {
                    case "product":
                        lastRefreshTime = userContexts[dataSet] as DateTime?;
                        datasetHasUpdated = HasProductDatasetUpdated(dataSet, lastRefreshTime);
                        if (datasetHasUpdated)
                        {
                            DatasetsNeedToRefresh.Add(dataSet, RefreshPreference.RefreshNow);
                        }
                        break;
                    case "customer":
                        lastRefreshTime = userContexts[dataSet] as DateTime?;
                        datasetHasUpdated = HasSupplierDatasetUpdated(dataSet, lastRefreshTime);
                        if (datasetHasUpdated)
                        {
                            DatasetsNeedToRefresh.Add(dataSet, RefreshPreference.RefreshOnNextTimeOfDay);
                        }
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Dataset.");
                }
            }

            return DatasetsNeedToRefresh;
        }

        public void Dispose()
        {
            // clean your unmanaged resources
        }

        private IList<object> FetchProductsFromDataSource()
        {
            string query = @"                
                    SELECT [id], [name]
                    FROM [dbo].[Product]
            ";
            return ExecuteQuery(query, "product");
        }

        private IList<object> FetchCustomersFromDataSource()
        {
            string Query = "select * from dbo.Customer";
            return ExecuteQuery(Query, "customer");

        }

        private IList<object> ExecuteQuery(string query, string dataset)
        {
            IList<object> data;

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    data = GetData(reader, dataset);
                }
            }
            return data;
        }

        private IList<object> GetData(SqlDataReader dataReader, string dataset)
        {
            IList<object> data = new List<object>();
            while (dataReader.Read())
            {
                switch (dataset)
                {
                    case "product":
                        Product product = new Product() { Id = dataReader.GetInt32(0), Name = dataReader.GetString(1) };
                        data.Add(product);
                        break;
                    case "customer":
                        User user = new User() { Id = dataReader.GetInt32(0), Name = dataReader.GetString(1) };
                        data.Add(user);
                        break;
                    default:
                        throw new InvalidOperationException("Invalid Dataset.");
                }
            }
            Console.WriteLine("Count in GetData: {0}", data.Count);
            return data;

        }

        public string[] GetKeys(IList<object> objects)
        {
            string[] keys = new string[objects.Count];
            for (int i = 0; i < keys.Length; i++)
            {
                keys[i] = objects[i].GetType() == typeof(Product) ? $"ProductId:{(objects[i] as Product).Id}" : $"CustomerId:{(objects[i] as User).Id}";
            }

            return keys;
        }

        public static IDictionary<string, CacheItem> GetCacheItemDictionary(string[] keys, IList<Object> value)
        {
            IDictionary<string, CacheItem> items = new Dictionary<string, CacheItem>();
            CacheItem cacheItem = null;

            for (int i = 0; i < value.Count; i++)
            {
                cacheItem = new CacheItem(value[i]);
                items.Add(keys[i], cacheItem);
            }

            return items;
        }

        private bool HasProductDatasetUpdated(string dataSet, object dateTime)
        {
            bool result = false;
            string query = $"select count(*) from dbo.Product where LastModify > '{dateTime as DateTime?}'";
            result = ExecuteAggregateQuery(query) > 0;
            return result;
        }

        private bool HasSupplierDatasetUpdated(string dataSet, object dateTime)
        {
            bool result = false;
            string query = $"select count(*) from dbo.Customer where LastModify > '{dateTime as DateTime?}'";
            result = ExecuteAggregateQuery(query) > 0;
            return result;
        }

        private IList<object> FetchUpdatedProducts(object dateTime)
        {
            string Query = $"select * from dbo.Product where LastModify > '{dateTime as DateTime?}'";
            return ExecuteQuery(Query, "product");

        }

        private IList<object> FetchUpdatedCustomers(object dateTime)
        {
            string Query = $"select * from dbo.Customer where LastModify > '{dateTime as DateTime?}'";
            return ExecuteQuery(Query, "customer");

        }

        private int ExecuteAggregateQuery(string Query)
        {
            int result = 0;
            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                SqlCommand oCmd = new SqlCommand(Query, myConnection);

                myConnection.Open();
                using (var reader = oCmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result = Convert.ToInt32(reader[0]);
                    }
                }
                myConnection.Close();
            }
            return result;
        }
    }
}
