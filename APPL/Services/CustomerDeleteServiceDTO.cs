using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class CustomerDeleteServiceDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
