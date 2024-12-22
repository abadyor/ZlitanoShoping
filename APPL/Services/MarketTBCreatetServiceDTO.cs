using System.ComponentModel.DataAnnotations;

namespace APPL.Services
{
    public class MarketTBCreatetServiceDTO
    {
        [Required]
        [StringLength(48)]
        public string Name { get; set; }

        [Required]
        [StringLength(8)]
        public string Scode { get; set; }

        [Required]
        [StringLength(200)]
        public string SInstance { get; set; }
    }
}
