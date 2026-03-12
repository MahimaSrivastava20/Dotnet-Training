using System.Collections.Generic;
using System.Linq;
namespace MvcCrudDemo.Models
{
    public class PersonRepository
    {
        public static List<Person> people = new List<Person>();
        public static int nextId = 1;
    }
}
