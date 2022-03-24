using System;
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
using System.Data;
using System.Data.SqlClient;


namespace Dependency
{
    public class DatabaseDependency
    {
        private static string connectionString = "Data Source=NOOR-UL-AIN\\SQLEXPRESS;Initial Catalog=CashAndCarry;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False;User ID=DIYATECH\\noor_ul_ain";
        private static ICache cache;
        public static void Run()
        { 
            Initialize();
            cache.Clear();
            GetDataFromDB();

            AddDataWithSQLDependency();
        }
        private static void Initialize()
        {
            cache = CacheManager.GetCache("PRCache");
            Console.WriteLine("Cache has been initialized!!!!\n");
        }

        private static void GetDataFromDB()
        {
            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                string query = "SELECT customer.id, Customer.name As 'Customer Name', Product.name, Order_Product.quantity " +
                    "FROM[CashAndCarry].[dbo].[Order] INNER JOIN[CashAndCarry].[dbo].[Customer] ON[CashAndCarry].[dbo].[Order].customerId = [CashAndCarry].[dbo].[Customer].id INNER JOIN[dbo].[Order_Product] on[dbo].[Order].[id] " +
                    "= [dbo].[Order_Product].[order_id] INNER JOIN Product on Order_Product.product_id = Product.id " +
                    "group by Order_Product.quantity, Customer.id, Customer.name, Product.name having SUM(Order_Product.quantity) > 2";
                SqlCommand cmd = new SqlCommand(query, conn);
                conn.Open();
                using(SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        //Console.Write(reader.GetInt32(0));
                        Console.Write(reader.GetString(1));
                        Console.Write(reader.GetString(2));
                        Console.WriteLine(reader.GetInt32(3));
                    }
                }
            }


        }

        private static void AddDataWithSQLDependency()
        {
            string query = "Select [dbo].[Customer].[id], [dbo].[Customer].[name] " +
                "from [dbo].[Customer] inner join [dbo].[Order] on [dbo].[Customer].[id] = [dbo].[Order].[customerId] " +
                "where [dbo].[Customer].[id] = 1";
            /*var param = new SqlCmdParams();
            param.Type = (CmdParamsType.Int);
            param.Value = 1;

            Dictionary<string, SqlCmdParams> sqlParams = new Dictionary<string, SqlCmdParams>();
            sqlParams.Add("@CustomerID", param);*/

            //var sqlDependency = new SqlCacheDependency(connectionString, query, SqlCommandType.Text, sqlParams);
            SqlCacheDependency sqlDependency = new SqlCacheDependency(connectionString, query);
            Order[] orders = GetOrders(1);
            foreach (Order order in orders)
            {
                // Generate a unique cache key for this order
                string key = $"Order:{order.Id}";

                CacheItem cacheItem = new CacheItem(order);
                cacheItem.Dependency = sqlDependency;

                cache.Add(key, cacheItem);
            }
        }

        private static Order[] GetOrders(int customerId)
        {
            List<Order> orders = new List<Order>();
            string query = "Select [dbo].[Order].[id] As 'Order Id' , [dbo].[Customer].[id] as 'Customer Id' " +
                "from [dbo].[Customer] inner join [dbo].[Order] on [dbo].[Customer].[id] = [dbo].[Order].[customerId] " +
                "where [dbo].[Customer].[id] = @CustomerID";
          

            using(SqlConnection conn = new SqlConnection(connectionString))
            {
                SqlCommand sqlCommand = new SqlCommand(query, conn);
                sqlCommand.Parameters.AddWithValue("@CustomerID", customerId);
                conn.Open();
                using(SqlDataReader reader = sqlCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int oID = reader.GetInt32(0);
                        int cID = reader.GetInt32(1);
                        orders.Add(new Order { Id = oID, CustomerId = cID });
                        Console.WriteLine(oID + "\t" +cID);
                    }
                }
            }
            return orders.ToArray();
        }

        private static void GetItemsFromCache()
        {

        }
    }
}
