using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Client;
using Alachisoft.NCache.Runtime.Exceptions;
using DataModel;

namespace NCachePractice
{
    public class Groups
    {
        private static ICache cache;
        public static void Run()
        {
            //Initialize Cache
            Initialize();

            // Add data in cache with groups
            AddItems();

            // Get Keys of group DOCOTR
            GetGroupKeys("doctor");

            //Get Keys of group SOFTWAR ENGINEER
            GetGroupKeys("SoftwareEngineer");

            //Get Keys of Group COMSATS TEACHER
            GetGroupKeys("ComsatsTeacher");

            //Get user data of group DOCTOR
            GetGroupData("Doctor");

            //Get user data of group SOFTWAR ENGINEER
            GetGroupData("SoftwareEngineer");

            //Get user data of Group COMSATS TEACHER
            GetGroupData("ComsatsTeacher");

            //Remove Group COMSATS TEACHER
            RemoveGroupData("ComsatsTeacher");
            GetGroupData("ComsatsTeacher");

            //Search User Group with SQL Query
            //SearchGroupWithSqlQuery("Saliha Azeem");
        }
        public static void Initialize()
        {
            cache = CacheManager.GetCache("ClusteredCache-2");
            cache.Clear();
            Console.WriteLine("Cache has been initialized!");
        }

        private static void AddItems()
        {
            //Add Doctors
            AddGroupedItemsInCache(1, "Dr. Zunnoorain", 30, "Doctor");
            AddGroupedItemsInCache(2, "Dr. Shehzad Roy", 33, "Doctor");
            AddGroupedItemsInCache(3, "Dr. Javeria", 26, "Doctor");

            //Add Software Engineers
            AddGroupedItemsInCache(1, "M. Noorulain Roy", 24, "SoftwareEngineer");
            AddGroupedItemsInCache(2, "Zeeshan Ali Hamdani", 24, "SoftwareEngineer");
            AddGroupedItemsInCache(3, "Zeqi Syed", 22, "SoftwareEngineer");

            //Add Comsats Teachers
            AddGroupedItemsInCache(1, "Dr. Fahad Munir", 37, "ComsatsTeacher");
            AddGroupedItemsInCache(2, "Qasim Malik", 37, "ComsatsTeacher");
            AddGroupedItemsInCache(3, "Zaheer ul Hassan", 40, "ComsatsTeacher");
        }

        private static void AddGroupedItemsInCache(int id, string userName, int age, string profession)
        {
            User user = new User(id, userName, age, profession);
            CacheItem item = new CacheItem(user);
            item.Group = profession;
            cache.Add((profession+""+id), item);
        }

        // Get Keys belonging to a specific group
        private static void GetGroupKeys(string group)
        {
            ICollection<string> keys = cache.SearchService.GetGroupKeys(group);
            if(keys != null && keys.Count > 0)
            {
                Console.WriteLine("Following keys retrieved against the group " + group);
                int index = 1;
                foreach(string key in keys)
                {
                    Console.WriteLine(index + ". " + key);
                }
            }
            else
            {
                Console.WriteLine("No key found against the group " + group);
            }
            Console.WriteLine("\n");
        }

        // Get keys as well as data of a certain group
        private static void GetGroupData(string group)
        {
            IDictionary<string, User> userData = cache.SearchService.GetGroupData<User>(group);
            Console.WriteLine("Following users found belonging to Group " + group);
            if (userData != null && userData.Count > 0)
            {
                foreach (KeyValuePair<string, User> user in userData)
                {
                    DisplayUserDetails(user.Value);
                }
            }
            else
            {
                Console.WriteLine("No user found against the group " + group);
            }
            Console.WriteLine("\n");
        }


        private static void DisplayUserDetails(User user)
        {
            Console.WriteLine("Name:       "+ user.Name);
            Console.WriteLine("Age:        "+ user.Age);
            Console.WriteLine("Profession: " + user.Group);
            Console.WriteLine("==========================================");
        }


        // Remove Group data from cache
        private static void RemoveGroupData(string group)
        {
            cache.SearchService.RemoveGroupData(group);
            Console.WriteLine("Group " + group + " has been removed from cache");
        }

        // Search for groups present in the cache with SQL queries
        private static void SearchGroupWithOqlQuery(string userName)
        {
            // Search for items with group
            // Provide Fully Qualified Name (FQN) of your custom class 
            string query = "SELECT $Group$ FROM FQN.User WHERE Name = ?";

            // Use QueryCommand for query execution
            var queryCommand = new QueryCommand(query);
            
            // Providing parameters for query
            queryCommand.Parameters.Add("Name", userName);

            // Executing QueryCommand through ICacheReader
            ICacheReader cacheReader = cache.SearchService.ExecuteReader(queryCommand);

            //Check if the result set is not empty
            if(cacheReader.FieldCount > 0)
            {
                Console.WriteLine("User " + userName + " belongs to the following group.");
                while (cacheReader.Read())
                {
                    string groupName = cacheReader.GetValue<string>(1);
                    Console.WriteLine(groupName);
                }
            }
        }
    }
}
