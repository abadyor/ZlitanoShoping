using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class VendorLoginusernamepasswordDTO
    {
        [Required]
        [StringLength(50)]
        public string Username { get; set; }

        [Required]
        [StringLength(32, MinimumLength = 6)]
        public string Password { get; set; }
    }
}
