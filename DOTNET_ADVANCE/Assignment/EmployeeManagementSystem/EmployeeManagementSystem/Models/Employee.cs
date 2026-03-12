using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementSystem.Models
{
    public class Employee
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        [StringLength(12)]
        public string AadhaarCard { get; set; }

        [Required]
        public DateTime DOB { get; set; }

        [Required]
        public decimal Salary { get; set; }
    }
}