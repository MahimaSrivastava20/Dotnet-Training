using System.ComponentModel.DataAnnotations;

namespace StudentReportCard.ViewModels
{
    public class LoginVM
    {
        [Required]
        public int Id { get; set; }

        [Required]
        public string Password { get; set; }
    }
}  
