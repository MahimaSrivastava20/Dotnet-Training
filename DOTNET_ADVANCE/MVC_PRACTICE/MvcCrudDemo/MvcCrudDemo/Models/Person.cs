using System.ComponentModel.DataAnnotations;

namespace MvcCrudDemo.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public string Name { get; set; }
    }
}
