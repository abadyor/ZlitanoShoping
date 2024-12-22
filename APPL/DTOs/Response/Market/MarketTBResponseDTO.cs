using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Response.Market
{
    public class MarketTBResponseDTO
    {
        public int Id { get; set; }

        [Required]
        [StringLength(40)]
        public string ProductName { get; set; }

        [StringLength(100)]
        public string Description { get; set; }

        public decimal Price { get; set; }
        public decimal DiscountedPrice { get; set; }
        public bool HasDiscount { get; set; }
        public DateTime TimeCreated { get; set; }
    }

}
