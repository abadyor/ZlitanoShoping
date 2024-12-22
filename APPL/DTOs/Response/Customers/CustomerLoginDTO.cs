using System.ComponentModel.DataAnnotations;

namespace APPL.DTOs.Response.Customers
{
    public class CustomerLoginDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
