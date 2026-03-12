using System.ComponentModel.DataAnnotations;

namespace MvcAdoCrud.Models
{
    public class Person
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Name can contain only letters and spaces")]
        public string Name { get; set; }
    }
}
