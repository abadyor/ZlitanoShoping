using System.ComponentModel.DataAnnotations;

namespace APPL.DTOs.Request.Customers
{
    public class CustomerDeleteRequestDTO
    {
        [Required]
        public int Id { get; set; }
    }
}
