using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace APPL.DTOs.Request.Market
{
    public class MarketTBCreatetDTO
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
