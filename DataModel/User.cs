using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModel
{
    [Serializable]
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public string Group { get; set; }

        public User() { }
        public User(string name, int age)
        {
            Name = name;
            Age = age;
        }

        public User(int id, string name, int age, string group)
        {
            Id = id;
            Name = name;
            Age = age;
            Group = group;
        }

        public User(int id, string name, int age)
        {
            Id = id;
            Name = name;
            Age = age;
        }

    }
}
