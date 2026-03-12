using System.ComponentModel.DataAnnotations;

namespace OnetoManyMVCDB.Models
{
    public class Customer
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public string City { get; set; }

        // One customer can have many orders
        public List<Order> Orders { get; set; }
    }
}
