using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.CacheLoader;
using System.Threading.Tasks;
using DataModel;
using System.Data.SqlClient;

namespace LoaderRefresher
{
    public class LoaderRefresher : ICacheLoader
    {
        private string connectionString;
        private static ICache cache;

        public void Init(IDictionary<string, string> parameters, string cacheName)
        {
            //System.Diagnostics.Debugger.Launch();
            //System.Diagnostics.Debugger.Break();
            /*connectionString = parameters["Conn"];
            cache = CacheManager.GetCache(cacheName);*/

            connectionString = @"Data Source=NOOR-UL-AIN\SQLEXPRESS;Initial Catalog=CashAndCarry;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User ID=DIYATECH\noor_ul_ain";
            cache = CacheManager.GetCache("ClusteredCache");
            //cache.Clear();
        }

        public object LoadDatasetOnStartup(string dataset)
        {
            //System.Diagnostics.Debugger.Launch();
            IList<object> datasetToLoad;

            switch (dataset)
            {
                case "products":
                    datasetToLoad = LoadProductsFromDatabase();
                    foreach (Product item in datasetToLoad)
                    {
                        CacheItem cacheItem = new CacheItem(item);
                        cache.Insert($"ProductId:{item.Id}", cacheItem);
                        cacheItem.ResyncOptions.
                    }
                    break;
                case "customers":
                    datasetToLoad = LoadCustomersFromDatabase();
                    foreach (User item in datasetToLoad)
                    {
                        CacheItem cacheItem = new CacheItem(item);
                        cache.Insert($"CustomerId:{item.Id}", cacheItem);
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid Dataset.");
            }

            object userContext = DateTime.Now;
            return userContext;
        }

        public object RefreshDataset(string dataset, object userContext)
        {
           return LoadDatasetOnStartup(dataset);
        }


        public IDictionary<string, RefreshPreference> GetDatasetsToRefresh(IDictionary<string, object> userContexts)
        {
            return null;
        }

        public void Dispose()
        {
            //System.Diagnostics.Debugger.Launch();
            //throw new NotImplementedException();
            this.Dispose();
        }

        private IList<object> LoadProductsFromDatabase()
        {
            string query = @"SELECT [id],[name]
                              FROM [CashAndCarry].[dbo].[Product]";
            return ExecuteQuery(query, "products");
        }

        private IList<object> LoadCustomersFromDatabase()
        {
            string query = @"SELECT [id],[name]
                              FROM [CashAndCarry].[dbo].[Customer]";
            return ExecuteQuery(query, "customers");
        }

        private IList<object> ExecuteQuery(string query, string dataset)
        {
            IList<object> data = new List<object>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(query, connection);
                connection.Open();
                using (var reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        switch (dataset)
                        {
                            case "products":
                                Product product = new Product() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                                data.Add(product);
                                break;
                            case "customers":
                                User customer = new User() { Id = reader.GetInt32(0), Name = reader.GetString(1) };
                                data.Add(customer);
                                break;
                            default: throw new NotImplementedException();
                        }
                    }
                }
            }
            return data;
        }

    }
}
