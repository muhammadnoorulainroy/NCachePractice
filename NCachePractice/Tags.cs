using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Alachisoft.NCache.Runtime.Caching;
using Alachisoft.NCache.Client;
using System.Configuration;
using DataModel;

namespace NCachePractice
{
    public class Tags
    {
        private static ICache cache;
        private static Tag[] tags;
        public static void Run()
        {
            Initialize();
            InitializeArray();
            AddUsersWithTags(tags);
            //GetUserDataByOneTag(tags[3]);

            GetByAnyTag(tags);
            //GetByAllTag(new Tag[2] { tags[0], tags[1] });

           // RemoveByAllTags(new Tag[2] { tags[0], tags[1] });
           
            RemoveByAnyTag(tags);

            GetByAnyTag(tags);
        }
        private static void Initialize()
        {
            cache = CacheManager.GetCache("ClusteredCache-2");
            cache.Clear();
            Console.WriteLine("Cache has been initialized!");
        }

        //initialize array with tags
        private static void InitializeArray()
        {
            tags = new Tag[14];
            tags[0] = new Tag("Doctor");
            tags[1] = new Tag("CardiacSurgeon");
            tags[2] = new Tag("OrthopedicSurgeon");
            tags[3] = new Tag("Physician");
            tags[4] = new Tag("Engineer");
            tags[5] = new Tag("SoftwareEngineer");
            tags[6] = new Tag("MechanicalEngineer");
            tags[7] = new Tag("Teacher");
            tags[8] = new Tag("SchoolTeacher");
            tags[9] = new Tag("CollegeTeacher");
            tags[10] = new Tag("UniversityTeacher");
            tags[11] = new Tag("BioTeacher");
            tags[12] = new Tag("MathTeacher");
            tags[13] = new Tag("HistoryTeacher");
        }

        private static void AddUsersWithTags(Tag[] tagList)
        {
            //Add Cardiac Surgeons in cache
            AddTaggedUsersInCache(1, "Dr. Shehzad Roy", 33, new Tag[2] { tags[0], tags[1] });
            AddTaggedUsersInCache(2, "Dr. Sajjad Kharal", 50, new Tag[2] { tags[0], tags[1] });

            //Add Orthopedic Surgeons
            AddTaggedUsersInCache(3, "Dr. Zunnoorain", 33, new Tag[2] { tags[0], tags[2] });
            AddTaggedUsersInCache(4, "Dr. Javeria", 50, new Tag[2] { tags[0], tags[2] });

            //Add Physcians
            AddTaggedUsersInCache(5, "Dr. Abdullah", 33, new Tag[2] { tags[0], tags[3] });
            AddTaggedUsersInCache(6, "Dr. Ahmad", 50, new Tag[2] { tags[0], tags[3] });

            //Add MBBS Doctors
            AddTaggedUsersInCache(7, "Dr. Abdul Wahab", 20, new Tag[1] { tags[0]});
            AddTaggedUsersInCache(8, "Dr. Noman", 23, new Tag[1] { tags[0] });
        }

        private static void AddTaggedUsersInCache(int id, string name, int age, Tag[] tagList)
        {
            CacheItem cacheItem = new CacheItem(new User(id, name, age));
            cacheItem.Tags = tagList;
            cache.Add(id+""+name, cacheItem);
        }

        private static void GetUserDataByOneTag(Tag tag)
        {
            IDictionary<string, User> data = cache.SearchService.GetByTag<User>(tag);
            if (data != null && data.Count>0)
            {
                Console.WriteLine("\nFollowing Users Found By Tag " + tag.TagName+"\n");
                foreach (KeyValuePair<string, User> item in data)
                {
                    DisplayUserDetails(item.Value);
                }
            }
            else
            {
                Console.WriteLine("No user found against the provided tag");
            }
        }

        private static void GetByAnyTag(Tag[] tags)
        {

            IDictionary<string, User> data = cache.SearchService.GetByTags<User>(tags, TagSearchOptions.ByAnyTag);
            if (data != null && data.Count > 0)
            {
                Console.WriteLine("\nFollowing Users Found through GetByAnyTag Method");
                foreach (KeyValuePair<string, User> item in data)
                {
                    DisplayUserDetails(item.Value);
                }
            }
            else
            {
                Console.WriteLine("No user found matching any of the provided tags");
            }
        }

        private static void GetByAllTag(Tag[] tags)
        {

            IDictionary<string, User> data = cache.SearchService.GetByTags<User>(tags, TagSearchOptions.ByAllTags);
            if (data != null && data.Count > 0 )
            {
                Console.WriteLine("\nFollowing Users Found through GetByAllTags Method");
                foreach (KeyValuePair<string, User> item in data)
                {
                    DisplayUserDetails(item.Value);
                }
            }
            else
            {
                Console.WriteLine("No user found that matches all of the provided tags");
            }
        }

        //Remove cache items which match the provided tag
        private static void RemoveByTag(Tag tag)
        {
            cache.SearchService.RemoveByTag(tag);
            Console.WriteLine("\nItems with tag " + tag.TagName + " have been removed");
        }

        //Remove cache items that match any one of the provided tags
        private static void RemoveByAnyTag(Tag[] tagList)
        {
            cache.SearchService.RemoveByTags(tagList, TagSearchOptions.ByAnyTag);
            Console.WriteLine("Items removed by RemoveByAnyTag Method");
        }

        //Remove cache items that match all of the provided tags
        private static void RemoveByAllTags(Tag[] tagList)
        {
            cache.SearchService.RemoveByTags(tagList, TagSearchOptions.ByAllTags);
            Console.WriteLine("Items removed by RemoveByAllTags Method");
        }
        private static void DisplayUserDetails(User user)
        {
            Console.WriteLine("Name:       " + user.Name);
            Console.WriteLine("Age:        " + user.Age);
            Console.WriteLine("==========================================");
        }
    }
}
