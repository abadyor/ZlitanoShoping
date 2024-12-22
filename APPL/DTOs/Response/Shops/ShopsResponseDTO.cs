using System.ComponentModel.DataAnnotations;

namespace APPL.DTOs.Response.Shops
{
    public class ShopsResponseDTO
    {


        public int Id { get; set; }
        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        [Required]
        public decimal Price { get; set; }

        public decimal DiscountedPrice { get; set; }
        public bool HasDiscount { get; set; }
    }

}
